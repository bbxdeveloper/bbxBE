using AutoMapper;
using AutoMapper.Configuration;
using bbxBE.Application.BLL;
using bbxBE.Application.Commands.cmdProduct;
using bbxBE.Common.Exceptions;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Queries.qProduct;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Enums;
using bbxBE.Domain.Entities;
using bxBE.Application.Commands.cmdProduct;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using bbxBE.Application.Commands.ResultModels;
using System.Globalization;
using Telerik.Reporting.Drawing;
using bbxBE.Application.Interfaces;

namespace bbxBE.Application.Commands.cmdImport
{
    public class ImportProductCommand : IRequest<Response<ImportedItemsStatistics>>
    {
        public List<IFormFile> ProductFiles { get; set; }
        public string FieldSeparator { get; set; } = ";";
    }

    public class ImportProductCommandHandler : ProductMappingParser, IRequestHandler<ImportProductCommand, Response<ImportedItemsStatistics>>
    {
        private const string DescriptionFieldName = "Description";
        private const string ProductGroupCodeFieldName = "ProductGroupCode";
        private const string OriginCodeFieldName = "OriginCode";
        private const string UnitOfMeasureFieldName = "UnitOfMeasure";
        private const string UnitPrice1FieldName = "UnitPrice1";
        private const string UnitPrice2FieldName = "UnitPrice2";
        private const string LatestSupplyPriceFieldName = "LatestSupplyPrice";
        private const string IsStockFieldName = "IsStock";
        private const string MinStockFieldName = "MinStock";
        private const string OrdUnitFieldName = "OrdUnit";
        private const string ProductFeeFieldName = "ProductFee";
        private const string ActiveFieldName = "Active";
        private const string ProductCodeFieldName = "ProductCode";
        private const string EANFieldName = "EAN";
        private const string VTSZFieldName = "VTSZ";
        private const string NODISCOUNTFieldName = "NODISCOUNT";

        private readonly IProductRepositoryAsync _productRepository;
        private readonly IProductGroupRepositoryAsync _productGroupRepository;
        private readonly IOriginRepositoryAsync _originRepository;

        private readonly ICacheService<Product> _productcacheService;
        private readonly ICacheService<ProductGroup> _productGroupCacheService;
        private readonly ICacheService<Origin> _originCacheService;
        private readonly ICacheService<VatRate> _vatRateCacheService;


        //private readonly IUnitOfMEasure
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        private List<CreateProductCommand> createProductCommands = new List<CreateProductCommand>();
        private List<UpdateProductCommand> updateProductCommands = new List<UpdateProductCommand>();
        private List<Origin> createableOriginCodes = new List<Origin>();
        private List<ProductGroup> createableProductGroupCodes = new List<ProductGroup>();


        public ImportProductCommandHandler(IProductRepositoryAsync productRepository,
                                            IProductGroupRepositoryAsync productGroupCodeRepository,
                                            IOriginRepositoryAsync originRepository,
                                            IMapper mapper,
                                            ILogger<ImportProductCommandHandler> logger,
                                              ICacheService<Product> productCacheService,
                                            ICacheService<ProductGroup> productGroupCacheService,
                                            ICacheService<Origin> originCacheService,
                                            ICacheService<VatRate> vatRateCacheService
          )
        { 
            _productRepository = productRepository;
            _productGroupRepository = productGroupCodeRepository;
            _originRepository = originRepository;
            _mapper = mapper;
            _logger = logger;
            _productcacheService = productCacheService;
            _productGroupCacheService = productGroupCacheService;
            _originCacheService = originCacheService;
            _vatRateCacheService = vatRateCacheService;
        }

        public async Task<Response<ImportedItemsStatistics>> Handle(ImportProductCommand request, CancellationToken cancellationToken)
        {
            var mappedProductColumns = new ProductMappingParser().GetProductMapping(request).ReCalculateIndexValues();
            var productItemsFromCSV = await GetProductItemsAsync(request, mappedProductColumns.productMap);
            var importProductResponse = new ImportedItemsStatistics { AllItemsCount = productItemsFromCSV.Count };
            var productCodes = new Dictionary<string, long>();

            // Get Products from Db/Cache and filter to OWN category.
            var pCodes = await _productRepository.GetAllProductsFromDBAsync();
            foreach (var item in pCodes)
            {
                if (item.ProductCodes != null)
                {
                    foreach (var item2 in item.ProductCodes)
                    {
                        if (item2.ProductCodeCategory == "OWN" && (!productCodes.ContainsKey(item2.ProductCodeValue)))
                            productCodes.Add(item2.ProductCodeValue, item2.ProductID);
                    }
                }
            }

            // create a Product or Update only
            foreach (var item in productItemsFromCSV)
            {
                if (!productCodes.ContainsKey(item.Value.ProductCode))
                {
                    createProductCommands.Add(item.Value);
                }
                else
                {
                    var updateProductCommand = _mapper.Map<UpdateProductCommand>(item.Value);
                    productCodes.TryGetValue(updateProductCommand.ProductCode, out long ID);
                    updateProductCommand.ID = ID;
                    updateProductCommands.Add(updateProductCommand);
                }
            }

            if (createProductCommands.Count > 0)
                await CreateProdcutItems(importProductResponse, cancellationToken);
            if (updateProductCommands.Count > 0)
                await UpdateProductItems(importProductResponse, cancellationToken);

            if (importProductResponse.HasErrorDuringImport)
            {
                throw new ImportParseException("Hiba az importálás közben. További infokért nézze meg a log-ot!");
            }
            importProductResponse.CreatedItemsCount = createProductCommands.Count;
            importProductResponse.UpdatedItemsCount = updateProductCommands.Count;

            return new Response<ImportedItemsStatistics>(importProductResponse);
        }

        private async Task UpdateProductItems(ImportedItemsStatistics importProductResponse, CancellationToken cancellationToken)
        {
            // fill update Product items in Update list
            for (int i = 0; i < updateProductCommands.Count; i++)
            {
                await CreateOrUpdateProductionAsync(updateProductCommands[i], cancellationToken);
            }

            // save Prodcuts list into DB. They need to update only
            try
            {
                await bllProduct.UpdateRangeAsynch(updateProductCommands, _productRepository, _mapper, cancellationToken);
            }
            catch (Exception ex)
            {
                LogToErrorsByException(importProductResponse, ex);
            }
        }

        private async Task CreateProdcutItems(ImportedItemsStatistics importProductResponse, CancellationToken cancellationToken)
        {
            // fill create Product items in Create list
            for (int i = 0; i < createProductCommands.Count; i++)
            {
                await CreateOrUpdateProductionAsync(createProductCommands[i], cancellationToken);
            }

            // create product groups into DB

            // create origin into DB
            await _originRepository.AddOriginRangeAsync(createableOriginCodes);

            await _productGroupRepository.AddProudctGroupRangeAsync(createableProductGroupCodes);

            // save Prodcuts list into DB. They need to create only
            try
            {
                await bllProduct.CreateRangeAsynch(createProductCommands, _productRepository, _mapper, cancellationToken);
            }
            catch (Exception ex)
            {
                LogToErrorsByException(importProductResponse, ex);
            }
        }

        private void LogToErrorsByException(ImportedItemsStatistics importProductResponse, Exception ex)
        {
            importProductResponse.HasErrorDuringImport = true;
            importProductResponse.ErroredItemssCount = +1;
            _logger.LogError(ex.Message);
        }

        private async Task CreateOrUpdateProductionAsync(object item, CancellationToken cancellationToken)
        {
            switch (item.GetType().Name)
            {
                case "CreateProductCommand":
                    {
                        await CreateProductGroupCodeIfNotExistsAsync(item, cancellationToken);

                        await CreateOriginCodeIfNotExistsAsync(item, cancellationToken);

                        UpdateUnitOfMeasureIfNotExists(item);
                        break;
                    }
                case "UpdateProductCommand":
                    {
                        UpdateUnitOfMeasureIfNotExists(item);
                        break;
                    }
                default:
                    break;
            }


            return;
        }

        private static void UpdateUnitOfMeasureIfNotExists(object item)
        {
            switch (item.GetType().Name)
            {
                case "CreateProductCommand":
                    {
                        (item as CreateProductCommand).UnitOfMeasure = Enum.GetName(typeof(enUnitOfMeasure),
                            GetUnitOfMeasureValueByEnum((item as CreateProductCommand).UnitOfMeasure.ToUpper()));
                        break;
                    }
                case "UpdateProductCommand":
                    {
                        (item as UpdateProductCommand).UnitOfMeasure = Enum.GetName(typeof(enUnitOfMeasure),
                            GetUnitOfMeasureValueByEnum((item as UpdateProductCommand).UnitOfMeasure.ToUpper()));
                        break;
                    }
                default:
                    break;
            }
        }

        private static enUnitOfMeasure GetUnitOfMeasureValueByEnum(string unitOfMeasureValue)
        {
            unitOfMeasureValue = unitOfMeasureValue.Replace("\"", "").Trim();

            if ((unitOfMeasureValue == "DB") || (unitOfMeasureValue == "DB."))
                return enUnitOfMeasure.PIECE;
            if ((unitOfMeasureValue == "KG") || ((unitOfMeasureValue == "KILOGRAM")))
                return enUnitOfMeasure.KILOGRAM;
            if ((unitOfMeasureValue == "CSOM") || (unitOfMeasureValue == "CSOM.") || (unitOfMeasureValue == "CS"))
                return enUnitOfMeasure.PACK;
            if ((unitOfMeasureValue == "FM") || ((unitOfMeasureValue == "FM.")))
                return enUnitOfMeasure.LINEAR_METER;
            if ((unitOfMeasureValue == "M") || (unitOfMeasureValue == "M.") || (unitOfMeasureValue == "METER"))
                return enUnitOfMeasure.METER;
            return enUnitOfMeasure.OWN;

        }

        private async Task CreateOriginCodeIfNotExistsAsync(object item, CancellationToken cancellationToken)
        {
            if (!String.IsNullOrEmpty((item as CreateProductCommand).OriginCode))
            {
                if (_originRepository.IsUniqueOriginCode((item as CreateProductCommand).OriginCode) && !createableOriginCodes.Any(b => b.OriginCode == (item as CreateProductCommand).OriginCode))
                {
                    createableOriginCodes.Add(new Origin
                    {
                        OriginCode = (item as CreateProductCommand).OriginCode,
                        OriginDescription = (item as CreateProductCommand).OriginCode
                    });
                }
            }
        }

        private async Task CreateProductGroupCodeIfNotExistsAsync(object item, CancellationToken cancellationToken)
        {

            if (_productGroupRepository.IsUniqueProductGroupCode((item as CreateProductCommand).ProductGroupCode) && !createableProductGroupCodes.Any(aa => aa.ProductGroupCode == (item as CreateProductCommand).ProductGroupCode))
            {
                createableProductGroupCodes.Add(new ProductGroup
                {
                    ProductGroupCode = (item as CreateProductCommand).ProductGroupCode,
                    ProductGroupDescription = (item as CreateProductCommand).ProductGroupCode
                });
            }
        }

        private void LogToErrorHandler(ImportedItemsStatistics importProduct, FluentValidation.Results.ValidationResult result)
        {
            importProduct.HasErrorDuringImport = true;
            importProduct.ErroredItemssCount = +1;
            LogToErrorsByValidation(result);
        }

        private void LogToErrorsByValidation(FluentValidation.Results.ValidationResult result)
        {
            foreach (var failure in result.Errors)
            {
                _logger.LogError("Property " + failure.PropertyName + " failed validation. Error was: " + failure.ErrorMessage);
            }
        }

        private static async Task<Dictionary<string, CreateProductCommand>> GetProductItemsAsync(ImportProductCommand request, Dictionary<string, int> productMapping)
        {
            var producItems = new Dictionary<string, CreateProductCommand>();
            using (var reader = new StreamReader(request.ProductFiles[1].OpenReadStream()))
            {
                string currentLine;
                while ((currentLine = await reader.ReadLineAsync()) != null)
                {
                    var p = GetProductFromCSV(currentLine, productMapping, request.FieldSeparator);
                    foreach (var item in p)
                    {
                        if ((item.Value.Active) && (!producItems.ContainsKey(item.Key)))
                        {
                            producItems.Add(item.Key, item.Value);
                        }
                    }
                }
            }

            return producItems;
        }

        private static Dictionary<string, CreateProductCommand> GetProductFromCSV(string currentLine, Dictionary<string, int> productMapper, string fieldSeparator)
        {
            string regExpPattern = $"{fieldSeparator}(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))";
            Regex regexp = new Regex(regExpPattern);
            string[] currentFieldsArray = regexp.Split(currentLine);

            var createPRoductCommand = new CreateProductCommand();

            //var ci = new System.Globalization.CultureInfo("en-GB");
            //var numberStyle = NumberStyles.AllowDecimalPoint;

            try
            {
                createPRoductCommand.Description = productMapper.ContainsKey(DescriptionFieldName) ? currentFieldsArray[productMapper[DescriptionFieldName]].Replace("\"", "").Trim() : null;
                createPRoductCommand.ProductCode = productMapper.ContainsKey(ProductCodeFieldName) ? currentFieldsArray[productMapper[ProductCodeFieldName]].Replace("\"", "").Trim() : null;
                createPRoductCommand.OriginCode = productMapper.ContainsKey(OriginCodeFieldName) ? currentFieldsArray[productMapper[OriginCodeFieldName]].Replace("\"", "").Trim() : null;
                createPRoductCommand.UnitOfMeasure = productMapper.ContainsKey(UnitOfMeasureFieldName) ? currentFieldsArray[productMapper[UnitOfMeasureFieldName]].Replace("\"", "").Trim() : null;
                createPRoductCommand.UnitPrice1 = productMapper.ContainsKey(UnitPrice1FieldName) ? decimal.Parse(currentFieldsArray[productMapper[UnitPrice1FieldName]]) : 0;
                createPRoductCommand.UnitPrice2 = productMapper.ContainsKey(UnitPrice2FieldName) ? decimal.Parse(currentFieldsArray[productMapper[UnitPrice2FieldName]]) : 0;
                createPRoductCommand.IsStock = productMapper.ContainsKey(IsStockFieldName) ? bool.Parse(currentFieldsArray[productMapper[IsStockFieldName]]) : true;
                createPRoductCommand.MinStock = productMapper.ContainsKey(MinStockFieldName) ?
                    string.IsNullOrEmpty(currentFieldsArray[productMapper[MinStockFieldName]]) ? 0 :
                    decimal.Parse(currentFieldsArray[productMapper[MinStockFieldName]]) : 0;
                createPRoductCommand.OrdUnit = productMapper.ContainsKey(OrdUnitFieldName) ? decimal.Parse(currentFieldsArray[productMapper[OrdUnitFieldName]]) : 0;
                createPRoductCommand.ProductFee = productMapper.ContainsKey(ProductFeeFieldName) ? decimal.Parse(currentFieldsArray[productMapper[ProductFeeFieldName]]) : 0;
                createPRoductCommand.Active = productMapper.ContainsKey(ActiveFieldName) ?
                    (currentFieldsArray[productMapper[ActiveFieldName]] == "1" || currentFieldsArray[productMapper[ActiveFieldName]] == "IGAZ" ?
                    true : false)
                    : false;
                createPRoductCommand.EAN = productMapper.ContainsKey(EANFieldName) ? currentFieldsArray[productMapper[EANFieldName]].Replace("\"", "").Trim() : null;
                createPRoductCommand.VTSZ = productMapper.ContainsKey(VTSZFieldName) ? currentFieldsArray[productMapper[VTSZFieldName]].Replace("\"", "").Trim() : null;
                createPRoductCommand.ProductGroupCode = productMapper.ContainsKey(ProductGroupCodeFieldName) ? currentFieldsArray[productMapper[ProductGroupCodeFieldName]].Replace("\"", "").Trim() : null;
                createPRoductCommand.VatRateCode = "27%";
                //createPRoductCommand.LatestSupplyPrice = productMapper.ContainsKey(LatestSupplyPriceFieldName) ? decimal.Parse(currentFieldsArray[productMapper[LatestSupplyPriceFieldName]]) : 0;
                createPRoductCommand.NoDiscount = (productMapper.ContainsKey(NODISCOUNTFieldName)) && (currentFieldsArray[productMapper[NODISCOUNTFieldName]].Equals("IGAZ"))
                        ? true : false;

                return new Dictionary<string, CreateProductCommand> {
                    {
                        productMapper.ContainsKey(ProductCodeFieldName) ? currentFieldsArray[productMapper[ProductCodeFieldName]] : null,
                        createPRoductCommand
                    }
                };
            }
            catch (System.Exception ex)
            {
                throw new Exception($"{ex.Message} - ProductCode: {currentFieldsArray[productMapper[ProductCodeFieldName]]}");
            }
        }
    }
}
