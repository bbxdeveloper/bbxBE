using AutoMapper;
using AutoMapper.Configuration;
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

        private readonly IProductRepositoryAsync _ProductRepository;
        private readonly IMapper _mapper;
        

        public ImportProductCommandHandler(IProductRepositoryAsync ProductRepository, IMapper mapper)
        {
            _ProductRepository = ProductRepository;
            _mapper = mapper;
        }

        public async Task<Response<ImportProduct>> Handle(ImportProductCommand request, CancellationToken cancellationToken)
        {
            var productMapping = await GetProductMappingAsync(request.ProductFiles[0]);
            var producItems = await GetProductItemsAsync(request, productMapping);

            var importProduct = new ImportProduct();
            importProduct.AllItemsCount = producItems.Count;

            foreach (var item in producItems)
            {
                var getProductByProductCode = new GetProductByProductCode() { ProductCode = item.Key };

                var prod = await _ProductRepository.GetProductByProductCodeAsync(getProductByProductCode);

                if (prod.Keys.Count == 0)
                {
                    var productCreateResponse = await bllProduct.CreateAsynch(item.Value, _ProductRepository, _mapper, cancellationToken);
                    importProduct.CreatedItemsCount += 1;
                }
                else
                {
                    var updateProductCommand = _mapper.Map<UpdateProductCommand>(item.Value);
                    var productUpdateResponse = await bllProduct.UpdateAsynch(updateProductCommand, _ProductRepository, _mapper, cancellationToken);
                    importProduct.UpdatedItemsCount += 1;
                }
            }

            return new Response<ImportProduct>(importProduct);
        }

        private static async Task<Dictionary<string, CreateProductCommand>> GetProductItemsAsync(ImportProductCommand request, Dictionary<string, int> productMapping)
        {
            var producItems = new Dictionary<string, CreateProductCommand>();
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

        private static Dictionary<string, CreateProductCommand> GetProductFromCSV(string currentLine, Dictionary<string, int> productMapper)
        {
            string[] currentFieldsArray = currentLine.Split(';');

            try
            {
                return new Dictionary<string, CreateProductCommand> {
                    {
                        productMapper.ContainsKey(ProductCodeFieldName) ? currentFieldsArray[productMapper[ProductCodeFieldName]] : null,
                        new CreateProductCommand
                        {
                            Description = productMapper.ContainsKey(DescriptionFieldName) ? currentFieldsArray[productMapper[DescriptionFieldName]] : null,
                            ProductCode = productMapper.ContainsKey(ProductCodeFieldName) ? currentFieldsArray[productMapper[ProductCodeFieldName]] : null,
                            OriginCode = productMapper.ContainsKey(OriginCodeFieldName) ? currentFieldsArray[productMapper[OriginCodeFieldName]] : null,
                            UnitOfMeasure = productMapper.ContainsKey(UnitOfMeasureFieldName) ? currentFieldsArray[productMapper[UnitOfMeasureFieldName]] : null,
                            UnitPrice1 = productMapper.ContainsKey(UnitPrice1FieldName) ? decimal.Parse(currentFieldsArray[productMapper[UnitPrice1FieldName]]) : 0,
                            UnitPrice2 = productMapper.ContainsKey(UnitPrice2FieldName) ? decimal.Parse(currentFieldsArray[productMapper[UnitPrice2FieldName]]) : 0,
                            LatestSupplyPrice = productMapper.ContainsKey(LatestSupplyPriceFieldName) ? decimal.Parse(currentFieldsArray[productMapper[LatestSupplyPriceFieldName]]) : 0,
                            IsStock = productMapper.ContainsKey(IsStockFieldName) ? bool.Parse(currentFieldsArray[productMapper[IsStockFieldName]]) : false,
                            MinStock = productMapper.ContainsKey(MinStockFieldName) ? decimal.Parse(currentFieldsArray[productMapper[MinStockFieldName]]) : 0,
                            OrdUnit = productMapper.ContainsKey(OrdUnitFieldName) ? decimal.Parse(currentFieldsArray[productMapper[OrdUnitFieldName]]) : 0,
                            ProductFee = productMapper.ContainsKey(ProductFeeFieldName) ? decimal.Parse(currentFieldsArray[productMapper[ProductFeeFieldName]]) : 0,
                            Active = productMapper.ContainsKey(ActiveFieldName) ? bool.Parse(currentFieldsArray[productMapper[ActiveFieldName]]) : false,
                            EAN = productMapper.ContainsKey(EANFieldName) ? currentFieldsArray[productMapper[EANFieldName]] : null,
                            VTSZ = productMapper.ContainsKey(VTSZFieldName) ? currentFieldsArray[productMapper[VTSZFieldName]] : null,
                            ProductGroupCode = productMapper.ContainsKey(ProductGroupCodeFieldName) ? currentFieldsArray[productMapper[ProductGroupCodeFieldName]] : null
                        }
                    }
                };
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
