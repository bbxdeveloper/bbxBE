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

namespace bbxBE.Application.Queries.qOrigin
{
    public class GetOrigin:  IRequest<Entity>
    {
        public long ID { get; set; }
  //      public string Fields { get; set; }
    }

    public class GetOriginHandler : IRequestHandler<GetOrigin, Entity>
    {
        private readonly IOriginRepositoryAsync _originRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public GetOriginHandler(IOriginRepositoryAsync originRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _originRepository = originRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<Entity> Handle(GetOrigin request, CancellationToken cancellationToken)
        {
            var validFilter = request;
            var pagination = request;
          

            // query based on filter
            var entityPositions =  _originRepository.GetOrigin(validFilter);
            var data = entityPositions.MapItemFieldsByMapToAnnotation<Origin>();

            // response wrapper
            return data;
        }
    }
}