using AutoMapper;
using AutoMapper.Configuration;
using bbxBE.Application.BLL;
using bbxBE.Application.Commands.cmdProduct;
using bbxBE.Application.Exceptions;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Queries.qProduct;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Enums;
using bbxBE.Domain.Entities;
using bxBE.Application.Commands.cmdProduct;
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

namespace bbxBE.Application.Commands.cmdImport
{
    public class ImportProductCommand : IRequest<Response<ImportProduct>>
    {
        public List<IFormFile> ProductFiles { get; set; }
        public string FieldSeparator { get; set; } = ";";
    }

    public class ImportProductCommandHandler : ProductMappingParser, IRequestHandler<ImportProductCommand, Response<ImportProduct>>
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

        private readonly IProductRepositoryAsync _productRepository;
        private readonly IProductGroupRepositoryAsync _productGroupRepository;
        private readonly IOriginRepositoryAsync _originRepository;
        //private readonly IUnitOfMEasure
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        private List<CreateProductCommand> createProductCommands = new List<CreateProductCommand>();
        private List<UpdateProductCommand> updateProductCommands = new List<UpdateProductCommand>();
        private List<string> createableOriginCodes = new List<string>();
        private List<string> createableProductGroupCodes = new List<string>();


        public ImportProductCommandHandler(IProductRepositoryAsync productRepository,
                                            IProductGroupRepositoryAsync productGroupCodeRepository,
                                            IOriginRepositoryAsync originRepository,
                                            IMapper mapper,
                                            ILogger<ImportProductCommandHandler> logger)
        {
            _productRepository = productRepository;
            _productGroupRepository = productGroupCodeRepository;
            _originRepository = originRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Response<ImportProduct>> Handle(ImportProductCommand request, CancellationToken cancellationToken)
        {
            var mappedProducts = new ProductMappingParser().GetProductMapping(request).ReCalculateIndexValues();
            var producItems = await GetProductItemsAsync(request, mappedProducts.productMap);
            var importProduct = new ImportProduct { AllItemsCount = producItems.Count };
            var productItems = new HashSet<string>();

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var pCodes = await _productRepository.GetAllProductsFromDBAsync();
            foreach (var item in pCodes)
            {
                if (item.ProductCodes != null)
                {
                    foreach (var item2 in item.ProductCodes)
                    {
                        if (item2.ProductCodeCategory == "OWN")
                            productItems.Add(item2.ProductCodeValue);
                    }
                }   
            }

            stopWatch.Stop();

            foreach (var item in producItems)
            {
                if (!productItems.Contains(item.Value.ProductCode))
                {
                    createProductCommands.Add(item.Value);
                    //await CreateOrUpdateProductionAsync(importProduct, item.Value, cancellationToken);
                }
                else
                {
                    var updateProductCommand = _mapper.Map<UpdateProductCommand>(item.Value);
                    updateProductCommands.Add(updateProductCommand);
                    
                    //prod.TryGetValue("ID", out object _i);
                    //updateProductCommand.ID = long.Parse(_i.ToString());


                    //updateProductCommand.ID = long.Parse(item.Key.ToString());
                    //await CreateOrUpdateProductionAsync(importProduct, updateProductCommand, cancellationToken);
                }

                //var prod = await _productRepository.GetProductByProductCodeAsync(new GetProductByProductCode() { ProductCode = item.Key });
            }

            stopWatch.Stop();


            stopWatch.Restart();

            createProductCommands.RemoveRange(10, (createProductCommands.Count - 10));

            for (int i = 0; i < createProductCommands.Count; i++)
            {
                stopWatch.Restart();
                CreateOrUpdateProductionAsync(importProduct, createProductCommands[i], cancellationToken);
                stopWatch.Stop();
            }

            stopWatch.Stop();

            stopWatch.Restart();

            await bllProduct.CreateRangeAsynch(createProductCommands, _productRepository, _mapper, cancellationToken);

            stopWatch.Stop();

            stopWatch.Restart();

            for (int i = 0; i < updateProductCommands.Count; i++)
            {
                //updateProductCommands[i].ID = long.Parse(item.Key.ToString());
                CreateOrUpdateProductionAsync(importProduct, updateProductCommands[i], cancellationToken);
            }

            stopWatch.Stop();

            stopWatch.Restart();

            await bllProduct.UpdateRangeAsynch(updateProductCommands, _productRepository, _mapper, cancellationToken);

            stopWatch.Stop();
            if (importProduct.HasErrorDuringImport)
            {
                //throw new ImportParseException("Hiba az importálás közben. További infokért nézze meg a log-ot!");
            }
            importProduct.CreatedItemsCount = createProductCommands.Count;
            importProduct.UpdatedItemsCount = updateProductCommands.Count;

            return new Response<ImportProduct>(importProduct);
        }

        private async Task CreateOrUpdateProductionAsync(ImportProduct importProduct, object item, CancellationToken cancellationToken)
        {
            switch (item.GetType().Name)
            {
                case "CreateProductCommand":
                    {
                        await CreateProductGroupCodeIfNotExistsAsync(item, cancellationToken);

                        await CreateOriginCodeIfNotExistsAsync(item, cancellationToken);

                        UpdateUnitOfMeasureIfNotExists(item);

                        //var validator = new createProductCommandValidator(_productRepository);
                        //var result = await validator.ValidateAsync(item as CreateProductCommand);
                        //if (!result.IsValid)
                        //{
                        //    LogToErrorHandler(importProduct, result);
                        //}
                        //else
                        //{
                        //    //createProductCommands.Add(item as CreateProductCommand);
                        //    //await bllProduct.CreateAsynch(item as CreateProductCommand, _productRepository, _mapper, cancellationToken);
                        //    //importProduct.CreatedItemsCount += 1;
                        //}
                        break;
                    }
                case "UpdateProductCommand":
                    {
                        UpdateUnitOfMeasureIfNotExists(item);

                        //var validator = new UpdateProductCommandValidator(_productRepository);
                        //var result = await validator.ValidateAsync(item as UpdateProductCommand);
                        //if (!result.IsValid)
                        //{
                        //    LogToErrorHandler(importProduct, result);
                        //}
                        //else
                        //{
                        //    //updateProductCommands.Add(item as UpdateProductCommand);
                        //    //await bllProduct.UpdateAsynch(item as UpdateProductCommand, _productRepository, _mapper, cancellationToken);
                        //    //importProduct.UpdatedItemsCount += 1;
                        //}
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
                //var IsUniqueOriginCode = _originRepository.IsUniqueOriginCode((item as CreateProductCommand).OriginCode);
                if (_originRepository.IsUniqueOriginCode((item as CreateProductCommand).OriginCode))
                {
                    createableOriginCodes.Add((item as CreateProductCommand).OriginCode);
                    //await bllOrigin.CreateAsync((item as CreateProductCommand).OriginCode, (item as CreateProductCommand).OriginCode, _originRepository, cancellationToken);
                }
            }
        }

        private async Task CreateProductGroupCodeIfNotExistsAsync(object item, CancellationToken cancellationToken)
        {
            //var IsUniqueProductGroupCode = _productGroupRepository.IsUniqueProductGroupCode((item as CreateProductCommand).ProductGroupCode);
            if (_productGroupRepository.IsUniqueProductGroupCode((item as CreateProductCommand).ProductGroupCode))
            {
                createableProductGroupCodes.Add((item as CreateProductCommand).ProductGroupCode);
                //await bllProductGroup.CreateAsync((item as CreateProductCommand).ProductGroupCode, (item as CreateProductCommand).ProductGroupCode, _productGroupRepository, cancellationToken);
            }
        }

        private void LogToErrorHandler(ImportProduct importProduct, FluentValidation.Results.ValidationResult result)
        {
            importProduct.HasErrorDuringImport = true;
            importProduct.ErroredItemssCount = +1;
            LogToErrors(result);
        }

        private void LogToErrors(FluentValidation.Results.ValidationResult result)
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
            //currentLine = currentLine.Replace("\"", "\'");
            //string[] currentFieldsArray = currentLine.Replace("\"", "").Trim().Split(fieldSeparator);
            string[] currentFieldsArray = regexp.Split(currentLine); // currentLine.Replace("\"", "").Trim().Split(fieldSeparator);

            var createPRoductCommand = new CreateProductCommand();

            try
            {    
                createPRoductCommand.Description = productMapper.ContainsKey(DescriptionFieldName) ? currentFieldsArray[productMapper[DescriptionFieldName]].Trim() : null;
                createPRoductCommand.ProductCode = productMapper.ContainsKey(ProductCodeFieldName) ? currentFieldsArray[productMapper[ProductCodeFieldName]].Trim() : null;
                createPRoductCommand.OriginCode = productMapper.ContainsKey(OriginCodeFieldName) ? currentFieldsArray[productMapper[OriginCodeFieldName]].Trim() : null;
                createPRoductCommand.UnitOfMeasure = productMapper.ContainsKey(UnitOfMeasureFieldName) ? currentFieldsArray[productMapper[UnitOfMeasureFieldName]].Trim() : null;
                createPRoductCommand.UnitPrice1 = productMapper.ContainsKey(UnitPrice1FieldName) ? decimal.Parse(currentFieldsArray[productMapper[UnitPrice1FieldName]]) : 0;
                createPRoductCommand.UnitPrice2 = productMapper.ContainsKey(UnitPrice2FieldName) ? decimal.Parse(currentFieldsArray[productMapper[UnitPrice2FieldName]]) : 0;
                createPRoductCommand.IsStock = productMapper.ContainsKey(IsStockFieldName) ? bool.Parse(currentFieldsArray[productMapper[IsStockFieldName]]) : true;
                createPRoductCommand.MinStock = productMapper.ContainsKey(MinStockFieldName) ? decimal.Parse(currentFieldsArray[productMapper[MinStockFieldName]]) : 0;
                createPRoductCommand.OrdUnit = productMapper.ContainsKey(OrdUnitFieldName) ? decimal.Parse(currentFieldsArray[productMapper[OrdUnitFieldName]]) : 0;
                createPRoductCommand.ProductFee = productMapper.ContainsKey(ProductFeeFieldName) ? decimal.Parse(currentFieldsArray[productMapper[ProductFeeFieldName]]) : 0;
                createPRoductCommand.Active = productMapper.ContainsKey(ActiveFieldName) ? (currentFieldsArray[productMapper[ActiveFieldName]] == "1" ? true : false) : false;
                createPRoductCommand.EAN = productMapper.ContainsKey(EANFieldName) ? currentFieldsArray[productMapper[EANFieldName]].Trim() : null;
                createPRoductCommand.VTSZ = productMapper.ContainsKey(VTSZFieldName) ? currentFieldsArray[productMapper[VTSZFieldName]].Trim() : null;
                createPRoductCommand.ProductGroupCode = productMapper.ContainsKey(ProductGroupCodeFieldName) ? currentFieldsArray[productMapper[ProductGroupCodeFieldName]].Trim() : null;
                createPRoductCommand.VatRateCode = "27%";
                //createPRoductCommand.LatestSupplyPrice = productMapper.ContainsKey(LatestSupplyPriceFieldName) ? decimal.Parse(currentFieldsArray[productMapper[LatestSupplyPriceFieldName]]) : 0;

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
