using bbxBE.Application.BLL;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Queries.qProduct;
using bbxBE.Application.Wrappers;
using bbxBE.Domain.Entities;
using bxBE.Application.Commands.cmdProduct;
using MediatR;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
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
    }

    public class ImportProductCommandHandler : IRequestHandler<ImportProductCommand, Response<ImportProduct>>
    {
        private const string DescriptionFieldName = "Description";
        private const string ProducGroupFieldName = "ProductGroupID";
        private const string OriginIdFieldName = "OriginID";
        private const string UnitOfMeasureFieldName = "UnitOfMeasure";
        private const string UnitPrice1FieldName = "UnitPrice1";
        private const string UnitPrice2FieldName = "UnitPrice2";
        private const string LatestSupplyPriceFieldName = "LatestSupplyPrice";
        private const string IsStockFieldName = "IsStock";
        private const string MinStockFieldName = "MinStock";
        private const string OrdUnitFieldName = "OrdUnit";
        private const string ProductFeeFieldName = "ProductFee";
        private const string ActiveFieldName = "Active";
        private const string IDFieldName = "ID";
        private const string NatureIndicatorFieldName = "NatureIndicator";
        private const string ProductCodeFieldName = "ProductCode";
        private readonly IProductRepositoryAsync _ProductRepository;

        public ImportProductCommandHandler(IProductRepositoryAsync ProductRepository)
        {
            _ProductRepository = ProductRepository;
        }

        public async Task<Response<ImportProduct>> Handle(ImportProductCommand request, CancellationToken cancellationToken)
        {
            var productMapping = await GetProductMappingAsync(request.ProductFiles[0]);
            var producItems = await GetProductItemsAsync(request, productMapping);

            foreach (var item in producItems)
            {
                var productHandler = await new GetProductByProductCodeHandler(_ProductRepository, null, null).Handle(new GetProductByProductCode() { ProductCode = item.Key }, cancellationToken);
                //var product = await productHandler.Handle();
                //TODO: check item is existing or not
                if (productHandler.Equals(null))
                {
                    //TODO: if item is new then create with default data
                    var createProductCommand = new CreateProductCommand();
                    var productCreateResponse = await bllProduct.CreateAsynch(createProductCommand, _ProductRepository, null, cancellationToken);
                }
                else
                {
                    //TODO: if existing: update
                    var updateProductCommand = new UpdateProductCommand();
                    var productUpdateResponse = await bllProduct.UpdateAsynch(updateProductCommand, _ProductRepository, null, cancellationToken);
                }

                //TODO: calculate statistical data
            }


            var importProduct = new ImportProduct();
            return new Response<ImportProduct>(importProduct);
        }

        private static async Task<Dictionary<string, Product>> GetProductItemsAsync(ImportProductCommand request, Dictionary<string, int> productMapping)
        {
            var producItems = new Dictionary<string, Product>();
            using (var reader = new StreamReader(request.ProductFiles[1].OpenReadStream()))
            {
                string currentLine;
                while ((currentLine = await reader.ReadLineAsync()) != null)
                {
                    var p = GetProductFromCSV(currentLine, productMapping);
                    foreach (var item in p)
                    {
                        producItems.Add(item.Key, item.Value);
                    }
                }
            }

            return producItems;
        }

        private static Dictionary<string, Product> GetProductFromCSV(string currentLine, Dictionary<string, int> productMapper)
        {
            string[] currentFieldsArray = currentLine.Split(';');

            try
            {
                return new Dictionary<string, Product> {
                    {
                        productMapper.ContainsKey(ProductCodeFieldName) ? currentFieldsArray[productMapper[ProductCodeFieldName]] : null,
                        new Product
                        {
                            Description = productMapper.ContainsKey(DescriptionFieldName) ? currentFieldsArray[productMapper[DescriptionFieldName]] : null,
                            ProductGroupID = productMapper.ContainsKey(ProducGroupFieldName) ? long.Parse(currentFieldsArray[productMapper[ProducGroupFieldName]]) : 0,
                            OriginID = productMapper.ContainsKey(OriginIdFieldName) ? long.Parse(currentFieldsArray[productMapper[OriginIdFieldName]]) : 0,
                            UnitOfMeasure = productMapper.ContainsKey(UnitOfMeasureFieldName) ? currentFieldsArray[productMapper[UnitOfMeasureFieldName]] : null,
                            UnitPrice1 = productMapper.ContainsKey(UnitPrice1FieldName) ? decimal.Parse(currentFieldsArray[productMapper[UnitPrice1FieldName]]) : 0,
                            UnitPrice2 = productMapper.ContainsKey(UnitPrice2FieldName) ? decimal.Parse(currentFieldsArray[productMapper[UnitPrice2FieldName]]) : 0,
                            LatestSupplyPrice = productMapper.ContainsKey(LatestSupplyPriceFieldName) ? decimal.Parse(currentFieldsArray[productMapper[LatestSupplyPriceFieldName]]) : 0,
                            IsStock = productMapper.ContainsKey(IsStockFieldName) ? bool.Parse(currentFieldsArray[productMapper[IsStockFieldName]]) : false,
                            MinStock = productMapper.ContainsKey(MinStockFieldName) ? decimal.Parse(currentFieldsArray[productMapper[MinStockFieldName]]) : 0,
                            OrdUnit = productMapper.ContainsKey(OrdUnitFieldName) ? decimal.Parse(currentFieldsArray[productMapper[OrdUnitFieldName]]) : 0,
                            ProductFee = productMapper.ContainsKey(ProductFeeFieldName) ? decimal.Parse(currentFieldsArray[productMapper[ProductFeeFieldName]]) : 0,
                            Active = productMapper.ContainsKey(ActiveFieldName) ? bool.Parse(currentFieldsArray[productMapper[ActiveFieldName]]) : false,
                            ID = productMapper.ContainsKey(IDFieldName) ? long.Parse(currentFieldsArray[productMapper[IDFieldName]]) : 0,
                            NatureIndicator = productMapper.ContainsKey(NatureIndicatorFieldName) ? currentFieldsArray[productMapper[NatureIndicatorFieldName]] : null
                        }
                    }
                };

                //    public string ProductCode { get; set; }
                //public string Description { get; set; }
                //public string ProductGroupCode { get; set; }
                //public string OriginCode { get; set; }
                //public string UnitOfMeasure { get; set; }
                //public decimal UnitPrice1 { get; set; }
                //public decimal UnitPrice2 { get; set; }
                //public decimal LatestSupplyPrice { get; set; }
                //public bool IsStock { get; set; }
                //public decimal MinStock { get; set; }
                //public decimal OrdUnit { get; set; }
                //public decimal ProductFee { get; set; }
                //public bool Active { get; set; }
                //public string VTSZ { get; set; }
                //public string EAN { get; set; }

            }
            catch (System.Exception ex)
            {
                throw ex;
            }

        }

        private static async Task<Dictionary<string, int>> GetProductMappingAsync(IFormFile mappingFile)
        {
            string s;
            using (var reader = new StreamReader(mappingFile.OpenReadStream()))
            {
                s = await reader.ReadToEndAsync();
            }
            return JsonConvert.DeserializeObject<Dictionary<string, int>>(s);
        }
    }
}
