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
    public class GetProduct:  IRequest<Entity>
    {
        public long ID { get; set; }
  //      public string Fields { get; set; }
    }

    public class GetProductHandler : IRequestHandler<GetProduct, Entity>
    {
        private readonly IProductRepositoryAsync _productRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public GetProductHandler(IProductRepositoryAsync productRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<Entity> Handle(GetProduct request, CancellationToken cancellationToken)
        {
            var validFilter = request;
            var pagination = request;
          

            // query based on filter
            var entityPositions = await _productRepository.GetProductReponseAsync(validFilter);
            var data = entityPositions.MapItemFieldsByMapToAnnotation<GetProductViewModel>();

            // response wrapper
            return data;
        }
    }
}