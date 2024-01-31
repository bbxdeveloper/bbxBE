using AutoMapper;
using bbxBE.Application.Commands.ResultModels;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Commands;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using bbxBE.Common.Exceptions;
using bbxBE.Common.ExpiringData;
using bbxBE.Domain.Entities;
using bxBE.Application.Commands.cmdProduct;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Commands.cmdImport
{


    public class ImportProductProc : IImportProductProc
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
        private const string SulyFieldName = "SULY";
        private const string FAFAFieldName = "FAFA";


        private const string ImportLockKey = "PRODIMPORT";
        private readonly IProductGlobalRepositoryAsync _productRepository;
        private readonly IProductGroupGlobalRepositoryAsync _productGroupRepository;
        private readonly IOriginGlobalRepositoryAsync _originRepository;

        private readonly ICacheService<Product> _productcacheService;
        private readonly ICacheService<ProductGroup> _productGroupCacheService;
        private readonly ICacheService<Origin> _originCacheService;
        private readonly ICacheService<VatRate> _vatRateCacheService;

        private readonly IExpiringData<ExpiringDataObject> _expiringData;


        //private readonly IUnitOfMEasure
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        private List<CreateProductCommand> createProductCommands = new List<CreateProductCommand>();
        private List<UpdateProductCommand> updateProductCommands = new List<UpdateProductCommand>();
        private List<Origin> createableOriginCodes = new List<Origin>();
        private List<ProductGroup> createableProductGroupCodes = new List<ProductGroup>();


        public ImportProductProc(IProductGlobalRepositoryAsync productGlobalRepository,
                                           IProductGroupGlobalRepositoryAsync productGroupCodeGlobalRepository,
                                           IOriginGlobalRepositoryAsync originGlobalRepository,
                                           IMapper mapper,
                                           ILogger logger,
                                           ICacheService<Product> productCacheService,
                                           ICacheService<ProductGroup> productGroupCacheService,
                                           ICacheService<Origin> originCacheService,
                                           ICacheService<VatRate> vatRateCacheService,
                                           IExpiringData<ExpiringDataObject> expiringData)
        {
            _productRepository = productGlobalRepository;
            _productGroupRepository = productGroupCodeGlobalRepository;
            _originRepository = originGlobalRepository;
            _mapper = mapper;
            _logger = logger;
            _productcacheService = productCacheService;
            _productGroupCacheService = productGroupCacheService;
            _originCacheService = originCacheService;
            _vatRateCacheService = vatRateCacheService;
            _expiringData = expiringData;
        }


        public async Task Process(string mapFileContent, string CSVContent, string FieldSeparator, string SessionID, bool onlyInsert, CancellationToken cancellationToken)
        {

            await _expiringData.AddOrUpdateItemAsync(ImportLockKey, "0/0", SessionID, TimeSpan.FromHours(2));

            try
            {
                await _expiringData.AddOrUpdateItemAsync(ImportLockKey, $"GetAllProductsFromDB...", SessionID, TimeSpan.FromHours(2));
                var productCodes = new Dictionary<string, long>();
                var pCodes = await _productRepository.GetAllProductsFromDBAsync();


                var mappedProductColumns = new ProductMappingParser().GetProductMapping(mapFileContent).ReCalculateIndexValues();
                await _expiringData.AddOrUpdateItemAsync(ImportLockKey, $"Parsing CSV...", SessionID, TimeSpan.FromHours(2));
                var productItemsFromCSV = await GetProductItemsAsync(CSVContent, FieldSeparator, mappedProductColumns.productMap, SessionID);
                var importProductResponse = new ImportedItemsStatistics { AllItemsCount = productItemsFromCSV.Count };


                // Get Products from Db/Cache and filter to OWN category.
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
                int counter = 0;
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

                    if (++counter % 1000 == 0)
                    {
                        await _expiringData.AddOrUpdateItemAsync(ImportLockKey, $"Insert/Update preprocessing:{counter}/{productItemsFromCSV.Count}", SessionID, TimeSpan.FromHours(2));
                    }
                }
                await _expiringData.AddOrUpdateItemAsync(ImportLockKey, "Write is processing...", SessionID, TimeSpan.FromHours(2));


                if (createProductCommands.Count > 0)
                {
                    await CreateProdcutItems(importProductResponse, cancellationToken);
                }
                if (!onlyInsert && updateProductCommands.Count > 0)
                {
                    await UpdateProductItems(importProductResponse, cancellationToken);
                }

                if (importProductResponse.HasErrorDuringImport)
                {

                    throw new ImportParseException("Hiba az importálás közben. További infokért nézze meg a log-ot!");
                }
                importProductResponse.CreatedItemsCount = createProductCommands.Count;
                importProductResponse.UpdatedItemsCount = updateProductCommands.Count;

                _logger.Information(String.Format(bbxBEConsts.PROD_IMPORT_RESULT,
                    importProductResponse.AllItemsCount,
                    importProductResponse.CreatedItemsCount,
                    importProductResponse.UpdatedItemsCount,
                    importProductResponse.ErroredItemsCount));

                await _expiringData.AddOrUpdateItemAsync(ImportLockKey, $"Refresh caches...", SessionID, TimeSpan.FromHours(2));

                //**********
                //* Caches *
                //**********
                await _productcacheService.RefreshCache();
                await _vatRateCacheService.RefreshCache();
                await _originCacheService.RefreshCache();
                await _productGroupCacheService.RefreshCache();

            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message, null);
                throw;
            }
            finally
            {
                await _expiringData.DeleteItemAsync(ImportLockKey);
            }
        }

        private async Task UpdateProductItems(ImportedItemsStatistics importProductResponse, CancellationToken cancellationToken)
        {

            // save Prodcuts list into DB. They need to update only
            try
            {
                // fill update Product items in Update list
                for (int i = 0; i < updateProductCommands.Count; i++)
                {
                    await CreateProductGroupCodeIfNotExistsAsync(updateProductCommands[i].ProductGroupCode, cancellationToken);
                    await CreateOriginCodeIfNotExistsAsync(updateProductCommands[i].OriginCode, cancellationToken);
                    UpdateUnitOfMeasureIfNotExists(updateProductCommands[i]);
                }
                if (createableOriginCodes.Count > 0)
                {
                    await _originRepository.AddOriginRangeAsync(createableOriginCodes);
                }

                if (createableProductGroupCodes.Count > 0)
                {
                    await _productGroupRepository.AddProudctGroupRangeAsync(createableProductGroupCodes);
                }

                await _productRepository.UpdateRangeAsynch(updateProductCommands, cancellationToken);
            }
            catch (Exception ex)
            {
                LogToErrorsByException(importProductResponse, ex);
            }
        }

        private async Task CreateProdcutItems(ImportedItemsStatistics importProductResponse, CancellationToken cancellationToken)
        {

            // save Prodcuts list into DB. They need to create only
            try
            {
                for (int i = 0; i < createProductCommands.Count; i++)
                {
                    await CreateProductGroupCodeIfNotExistsAsync(createProductCommands[i].ProductGroupCode, cancellationToken);
                    await CreateOriginCodeIfNotExistsAsync(createProductCommands[i].OriginCode, cancellationToken);
                    UpdateUnitOfMeasureIfNotExists(createProductCommands[i]);
                }
                if (createableOriginCodes.Count > 0)
                {
                    await _originRepository.AddOriginRangeAsync(createableOriginCodes);
                }
                if (createableProductGroupCodes.Count > 0)
                {
                    await _productGroupRepository.AddProudctGroupRangeAsync(createableProductGroupCodes);
                }

                await _productRepository.CreateRangeAsynch(createProductCommands, cancellationToken);
            }
            catch (Exception ex)
            {
                LogToErrorsByException(importProductResponse, ex);
            }
        }

        private void LogToErrorsByException(ImportedItemsStatistics importProductResponse, Exception ex)
        {
            importProductResponse.HasErrorDuringImport = true;
            importProductResponse.ErroredItemsCount = +1;
            _logger.Error(ex.Message);
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

        private async Task CreateOriginCodeIfNotExistsAsync(string OriginCode, CancellationToken cancellationToken)
        {
            if (!String.IsNullOrEmpty(OriginCode))
            {
                if (_originRepository.IsUniqueOriginCode(OriginCode) && !createableOriginCodes.Any(b => b.OriginCode == OriginCode))
                {
                    createableOriginCodes.Add(new Origin
                    {
                        OriginCode = OriginCode,
                        OriginDescription = OriginCode
                    });
                }
            }
        }

        private async Task CreateProductGroupCodeIfNotExistsAsync(string ProductGroupCode, CancellationToken cancellationToken)
        {

            if (_productGroupRepository.IsUniqueProductGroupCode(ProductGroupCode) && !createableProductGroupCodes.Any(aa => aa.ProductGroupCode == ProductGroupCode))
            {
                createableProductGroupCodes.Add(new ProductGroup
                {
                    ProductGroupCode = ProductGroupCode,
                    ProductGroupDescription = ProductGroupCode
                });
            }
        }

        private void LogToErrorHandler(ImportedItemsStatistics importProduct, FluentValidation.Results.ValidationResult result)
        {
            importProduct.HasErrorDuringImport = true;
            importProduct.ErroredItemsCount = +1;
            LogToErrorsByValidation(result);
        }

        private void LogToErrorsByValidation(FluentValidation.Results.ValidationResult result)
        {
            foreach (var failure in result.Errors)
            {
                _logger.Error("Property " + failure.PropertyName + " failed validation. Error was: " + failure.ErrorMessage);
            }
        }

        private async Task<Dictionary<string, CreateProductCommand>> GetProductItemsAsync(string CSVContent, string FieldSeparator, Dictionary<string, int> productMapping, string sessionID)
        {
            var producItems = new Dictionary<string, CreateProductCommand>();
            List<String> lines = CSVContent.Split('\n').ToList();
            int counter = 0;
            lines.ForEach(async currentLine =>
            {
                if (!string.IsNullOrWhiteSpace(currentLine))
                {

                    Debug.WriteLine(currentLine);
                    if (++counter % 1000 == 0)
                    {
                        await _expiringData.AddOrUpdateItemAsync(ImportLockKey, $"Parsing CSV:{counter}/{lines.Count} {currentLine}", sessionID, TimeSpan.FromHours(2));
                    }
                    try
                    {
                        var p = GetProductFromCSV(currentLine, productMapping, FieldSeparator);
                        foreach (var item in p)
                        {
                            if ((item.Value.Active) && (!producItems.ContainsKey(item.Key)))
                            {
                                producItems.Add(item.Key, item.Value);
                            }
                        }
                    }
                    catch
                    {
                        throw;
                    }
                }
            });

            Debug.WriteLine("CSV parsing has done");
            return producItems;
        }

        private Dictionary<string, CreateProductCommand> GetProductFromCSV(string currentLine, Dictionary<string, int> productMapper, string fieldSeparator)
        {
            string regExpPattern = $"{fieldSeparator}(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))";
            Regex regexp = new Regex(regExpPattern);
            string[] currentFieldsArray = regexp.Split(currentLine);

            var createProductCommand = new CreateProductCommand();

            //var ci = new System.Globalization.CultureInfo("en-GB");
            //var numberStyle = NumberStyles.AllowDecimalPoint;

            try
            {
                createProductCommand.Description = productMapper.ContainsKey(DescriptionFieldName) ? currentFieldsArray[productMapper[DescriptionFieldName]].Replace("\"", "").Trim() : null;
                createProductCommand.ProductCode = productMapper.ContainsKey(ProductCodeFieldName) ? currentFieldsArray[productMapper[ProductCodeFieldName]].Replace("\"", "").Trim() : null;
                createProductCommand.OriginCode = productMapper.ContainsKey(OriginCodeFieldName) ? currentFieldsArray[productMapper[OriginCodeFieldName]].Replace("\"", "").Trim() : null;
                createProductCommand.UnitOfMeasure = productMapper.ContainsKey(UnitOfMeasureFieldName) ? currentFieldsArray[productMapper[UnitOfMeasureFieldName]].Replace("\"", "").Trim() : null;
                createProductCommand.UnitPrice1 = productMapper.ContainsKey(UnitPrice1FieldName) ? decimal.Parse(currentFieldsArray[productMapper[UnitPrice1FieldName]]) : 0;
                createProductCommand.UnitPrice2 = productMapper.ContainsKey(UnitPrice2FieldName) ? decimal.Parse(currentFieldsArray[productMapper[UnitPrice2FieldName]]) : 0;
                createProductCommand.IsStock = productMapper.ContainsKey(IsStockFieldName) ? bool.Parse(currentFieldsArray[productMapper[IsStockFieldName]]) : true;
                createProductCommand.MinStock = productMapper.ContainsKey(MinStockFieldName) ?
                    string.IsNullOrEmpty(currentFieldsArray[productMapper[MinStockFieldName]]) ? 0 :
                    decimal.Parse(currentFieldsArray[productMapper[MinStockFieldName]]) : 0;
                createProductCommand.OrdUnit = productMapper.ContainsKey(OrdUnitFieldName) ? decimal.Parse(currentFieldsArray[productMapper[OrdUnitFieldName]]) : 0;
                createProductCommand.ProductFee = productMapper.ContainsKey(ProductFeeFieldName) ? decimal.Parse(currentFieldsArray[productMapper[ProductFeeFieldName]]) : 0;
                createProductCommand.Active = productMapper.ContainsKey(ActiveFieldName) ?
                    (currentFieldsArray[productMapper[ActiveFieldName]] == "1" || currentFieldsArray[productMapper[ActiveFieldName]] == "IGAZ" ?
                    true : false)
                    : false;
                createProductCommand.EAN = productMapper.ContainsKey(EANFieldName) ? currentFieldsArray[productMapper[EANFieldName]].Replace("\"", "").Trim() : null;
                createProductCommand.VTSZ = productMapper.ContainsKey(VTSZFieldName) ? currentFieldsArray[productMapper[VTSZFieldName]].Replace("\"", "").Trim() : null;
                createProductCommand.ProductGroupCode = productMapper.ContainsKey(ProductGroupCodeFieldName) ? currentFieldsArray[productMapper[ProductGroupCodeFieldName]].Replace("\"", "").Trim() : null;
                if (string.IsNullOrWhiteSpace(createProductCommand.ProductGroupCode))
                {
                    createProductCommand.ProductGroupCode = createProductCommand.ProductCode.Substring(0, 3);
                }

                var FAFA = productMapper.ContainsKey(FAFAFieldName) ?
                    (currentFieldsArray[productMapper[FAFAFieldName]] == "1" || currentFieldsArray[productMapper[FAFAFieldName]] == "IGAZ" ?
                    true : false)
                    : false;

                if (FAFA)
                {
                    createProductCommand.VatRateCode = "FA";
                }
                else
                {
                    createProductCommand.VatRateCode = "27%";
                }
                //createPRoductCommand.LatestSupplyPrice = productMapper.ContainsKey(LatestSupplyPriceFieldName) ? decimal.Parse(currentFieldsArray[productMapper[LatestSupplyPriceFieldName]]) : 0;
                createProductCommand.NoDiscount = (productMapper.ContainsKey(NODISCOUNTFieldName)) && (currentFieldsArray[productMapper[NODISCOUNTFieldName]].Equals("IGAZ"))
                        ? true : false;
                createProductCommand.LatestSupplyPrice = productMapper.ContainsKey(LatestSupplyPriceFieldName) ? decimal.Parse(currentFieldsArray[productMapper[LatestSupplyPriceFieldName]]) : 0;
                createProductCommand.UnitWeight = productMapper.ContainsKey(SulyFieldName) ? decimal.Parse(currentFieldsArray[productMapper[SulyFieldName]]) : 0;
                return new Dictionary<string, CreateProductCommand> {
                    {
                        productMapper.ContainsKey(ProductCodeFieldName) ? currentFieldsArray[productMapper[ProductCodeFieldName]] : null,
                        createProductCommand
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
