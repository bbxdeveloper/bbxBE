using bbxBE.Application.BLL;
using bbxBE.Application.Interfaces.Repositories;
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
        private readonly IProductRepositoryAsync _ProductRepository;

        public ImportProductCommandHandler(IProductRepositoryAsync ProductRepository)
        {
            _ProductRepository = ProductRepository;
        }

        public async Task<Response<ImportProduct>> Handle(ImportProductCommand request, CancellationToken cancellationToken)
        {
            var productMapping = await GetProductMappingAsync(request.ProductFiles[0]);

            var producItems = new List<Product>();
            using (var reader = new StreamReader(request.ProductFiles[1].OpenReadStream()))
            {
                //new CsvHelper.CsvReader(fs);
                while (reader.Peek() >= 0)
                {
                    producItems.Add(await GetProductFromCSVAsync(reader));
                }
            }

            //TODO: check item is existing or not
            
            //TODO: if existing: update
            var updateProductCommand = new UpdateProductCommand();
            var prod = await bllProduct.UpdateAsynch(updateProductCommand, _ProductRepository, null, cancellationToken);
            
            //TODO: if item is new then create with default data

            //TODO: calculate statistical data

            var importProduct = new ImportProduct();
            return new Response<ImportProduct>(importProduct);
        }

        private static async Task<Product> GetProductFromCSVAsync(StreamReader reader)
        {
            return new Product
            {
                Description = await reader.
        };
                //await reader.ReadLineAsync();

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
