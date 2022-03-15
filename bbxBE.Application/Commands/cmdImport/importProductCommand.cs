using bbxBE.Application.BLL;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Domain.Entities;
using bxBE.Application.Commands.cmdProduct;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Commands.cmdImport
{
    public class ImportProductCommand : IRequest<Response<ImportProduct>>
    {
        public IFormFile ProductFile { get; set; }
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

            //TODO: file length validation

            //TODO: get file content

            //TODO: get mapping

            //TODO: parse line by line
            //TODO: check item is existing or not
            var updateProductCommand = new UpdateProductCommand();
            var prod = await bllProduct.UpdateAsynch(updateProductCommand, _ProductRepository, null, cancellationToken);
            //TODO: if existing: update
            //TODO: if item is new then create with default data

            //TODO: calculate statistical data

            var importProduct = new ImportProduct();
            return new Response<ImportProduct>(importProduct);
        }

    }
}
