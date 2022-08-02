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

namespace bbxBE.Application.Queries.qProductGroup
{
    public class GetProductGroup:  IRequest<Entity>
    {
        public long ID { get; set; }
  //      public string Fields { get; set; }
    }

    public class GetProductGroupHandler : IRequestHandler<GetProductGroup, Entity>
    {
        private readonly IProductGroupRepositoryAsync _productGroupRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public GetProductGroupHandler(IProductGroupRepositoryAsync productGroupRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _productGroupRepository = productGroupRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<Entity> Handle(GetProductGroup request, CancellationToken cancellationToken)
        {
            var validFilter = request;
            var pagination = request;
          

            // query based on filter
            var entity = _productGroupRepository.GetProductGroup(validFilter);
            var data = entity.MapItemFieldsByMapToAnnotation<GetProductGroupViewModel>();

            // response wrapper
            return data;
        }
    }
}