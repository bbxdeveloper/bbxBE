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
using System.IO;
using System.Text;
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

            foreach (var item in producItems)
            {
                var prod = await _productRepository.GetProductByProductCodeAsync(new GetProductByProductCode() { ProductCode = item.Key });

                if (prod.Keys.Count == 0)
                {
                    await CreateOrUpdateProductionAsync(importProduct, item.Value, cancellationToken);
                }
                else
                {
                    var updateProductCommand = _mapper.Map<UpdateProductCommand>(item.Value);
                    prod.TryGetValue("ID", out object _i);
                    updateProductCommand.ID = long.Parse(_i.ToString());
                    await CreateOrUpdateProductionAsync(importProduct, updateProductCommand, cancellationToken);
                }
            }

            if (importProduct.HasErrorDuringImport)
            {
                throw new ImportParseException("Hiba az importálás közben. További infokért nézze meg a log-ot!");
            }

            return new Response<ImportProduct>(importProduct);
        }

        private async Task CreateOrUpdateProductionAsync(ImportProduct importProduct, object item, CancellationToken cancellationToken)
        {
            switch (item.GetType().Name)
            {
                case "CreateProductCommand":
                    {
                        await CreateProductGroupCodeIfNotExists(item, cancellationToken);

                        await CreateOriginCodeIfNotExists(item, cancellationToken);

                        UpdateUnitOfMeasureIfNotExists(item);

                        var validator = new createProductCommandValidator(_productRepository);
                        var result = await validator.ValidateAsync(item as CreateProductCommand);
                        if (!result.IsValid)
                        {
                            LogToErrorHandler(importProduct, result);
                        }
                        else
                        {
                            await bllProduct.CreateAsynch(item as CreateProductCommand, _productRepository, _mapper, cancellationToken);
                            importProduct.CreatedItemsCount += 1;
                        }
                        break;
                    }
                case "UpdateProductCommand":
                    {
                        UpdateUnitOfMeasureIfNotExists(item);

                        var validator = new UpdateProductCommandValidator(_productRepository);
                        var result = await validator.ValidateAsync(item as UpdateProductCommand);
                        if (!result.IsValid)
                        {
                            LogToErrorHandler(importProduct, result);
                        }
                        else
                        {
                            await bllProduct.UpdateAsynch(item as UpdateProductCommand, _productRepository, _mapper, cancellationToken);
                            importProduct.UpdatedItemsCount += 1;
                        }
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
                        if (!Enum.TryParse((item as CreateProductCommand).UnitOfMeasure.ToUpper(), out enUnitOfMeasure uom))
                        {
                            (item as CreateProductCommand).UnitOfMeasure = Enum.GetName(typeof(enUnitOfMeasure), enUnitOfMeasure.OWN);
                        }
                        break;
                    }
                case "UpdateProductCommand":
                    {
                        if (!Enum.TryParse((item as UpdateProductCommand).UnitOfMeasure.ToUpper(), out enUnitOfMeasure uom))
                        {
                            (item as UpdateProductCommand).UnitOfMeasure = Enum.GetName(typeof(enUnitOfMeasure), enUnitOfMeasure.OWN);
                        }
                        break;
                    }
                default:
                    break;
            }
        }

        private async Task CreateOriginCodeIfNotExists(object item, CancellationToken cancellationToken)
        {
            var IsUniqueOriginCode = await _originRepository.IsUniqueOriginCodeAsync((item as CreateProductCommand).OriginCode);
            if (IsUniqueOriginCode)
            {
                await bllOrigin.CreateAsync((item as CreateProductCommand).OriginCode, (item as CreateProductCommand).OriginCode, _originRepository, cancellationToken);
            }
        }

        private async Task CreateProductGroupCodeIfNotExists(object item, CancellationToken cancellationToken)
        {
            var IsUniqueProductGroupCode = await _productGroupRepository.IsUniqueProductGroupCodeAsync((item as CreateProductCommand).ProductGroupCode);
            if (IsUniqueProductGroupCode)
            {
                await bllProductGroup.CreateAsync((item as CreateProductCommand).ProductGroupCode, (item as CreateProductCommand).ProductGroupCode, _productGroupRepository, cancellationToken);
            }
        }

        private void LogToErrorHandler(ImportProduct importProduct, FluentValidation.Results.ValidationResult result)
        {
            importProduct.HasErrorDuringImport = true;
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
                        if (item.Value.Active)
                            producItems.Add(item.Key, item.Value);
                    }
                }
            }

            return producItems;
        }

        private static Dictionary<string, CreateProductCommand> GetProductFromCSV(string currentLine, Dictionary<string, int> productMapper, string fieldSeparator)
        {
            string[] currentFieldsArray = currentLine.Replace("\"", "").Trim().Split(fieldSeparator);

            try
            {
                return new Dictionary<string, CreateProductCommand> {
                    {
                        productMapper.ContainsKey(ProductCodeFieldName) ? currentFieldsArray[productMapper[ProductCodeFieldName]] : null,
                        new CreateProductCommand
                        {
                            Description = productMapper.ContainsKey(DescriptionFieldName) ? currentFieldsArray[productMapper[DescriptionFieldName]].Trim() : null,
                            ProductCode = productMapper.ContainsKey(ProductCodeFieldName) ? currentFieldsArray[productMapper[ProductCodeFieldName]].Trim() : null,
                            OriginCode = productMapper.ContainsKey(OriginCodeFieldName) ? currentFieldsArray[productMapper[OriginCodeFieldName]].Trim() : null,
                            UnitOfMeasure = productMapper.ContainsKey(UnitOfMeasureFieldName) ? currentFieldsArray[productMapper[UnitOfMeasureFieldName]].Trim() : null,
                            UnitPrice1 = productMapper.ContainsKey(UnitPrice1FieldName) ? decimal.Parse(currentFieldsArray[productMapper[UnitPrice1FieldName]]) : 0,
                            UnitPrice2 = productMapper.ContainsKey(UnitPrice2FieldName) ? decimal.Parse(currentFieldsArray[productMapper[UnitPrice2FieldName]]) : 0,
                            //LatestSupplyPrice = productMapper.ContainsKey(LatestSupplyPriceFieldName) ? decimal.Parse(currentFieldsArray[productMapper[LatestSupplyPriceFieldName]]) : 0,
                            IsStock = productMapper.ContainsKey(IsStockFieldName) ? bool.Parse(currentFieldsArray[productMapper[IsStockFieldName]]) : true,
                            MinStock = productMapper.ContainsKey(MinStockFieldName) ? decimal.Parse(currentFieldsArray[productMapper[MinStockFieldName]]) : 0,
                            OrdUnit = productMapper.ContainsKey(OrdUnitFieldName) ? decimal.Parse(currentFieldsArray[productMapper[OrdUnitFieldName]]) : 0,
                            ProductFee = productMapper.ContainsKey(ProductFeeFieldName) ? decimal.Parse(currentFieldsArray[productMapper[ProductFeeFieldName]]) : 0,
                            Active = productMapper.ContainsKey(ActiveFieldName) ? (currentFieldsArray[productMapper[ActiveFieldName]] == "1" ? true : false) : false,
                            EAN = productMapper.ContainsKey(EANFieldName) ? currentFieldsArray[productMapper[EANFieldName]].Trim() : null,
                            VTSZ = productMapper.ContainsKey(VTSZFieldName) ? currentFieldsArray[productMapper[VTSZFieldName]].Trim() : null,
                            ProductGroupCode = productMapper.ContainsKey(ProductGroupCodeFieldName) ? currentFieldsArray[productMapper[ProductGroupCodeFieldName]].Trim() : null,
                            VatRateCode = "27%"
                        }
                    }
                };
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }
}
