using AutoMapper;
using MediatR;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Parameters;
using bbxBE.Application.Wrappers;
using bbxBE.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Domain.Extensions;
using bbxBE.Application.Queries.ViewModels;

namespace bbxBE.Application.Queries.qProduct
{
    public class GetProductByProductCode :  IRequest<Entity>
    {
        public string ProductCode { get; set; }
  //      public string Fields { get; set; }
    }

    public class GetProductByProductCodeHandler : IRequestHandler<GetProductByProductCode, Entity>
    {
        private readonly IProductRepositoryAsync _productRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public GetProductByProductCodeHandler(IProductRepositoryAsync productRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<Entity> Handle(GetProductByProductCode request, CancellationToken cancellationToken)
        {
            var validFilter = request;
            var pagination = request;
          

            // query based on filter
            var prod = await _productRepository.GetProductByProductCodeAsync(validFilter);
            var data = prod.MapItemFieldsByMapToAnnotation<GetProductViewModel>();

            // response wrapper
            return data;
        }
    }
}