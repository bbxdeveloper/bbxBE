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

namespace bbxBE.Application.Queries.qLocation
{
    public class GetLocation:  IRequest<Entity>
    {
        public long ID { get; set; }
  //      public string Fields { get; set; }
    }

    public class GetLocationHandler : IRequestHandler<GetLocation, Entity>
    {
        private readonly ILocationRepositoryAsync _LocationRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public GetLocationHandler(ILocationRepositoryAsync LocationRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _LocationRepository = LocationRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<Entity> Handle(GetLocation request, CancellationToken cancellationToken)
        {
            var validFilter = request;
            var pagination = request;
          

            // query based on filter
            var entity = await _LocationRepository.GetLocationAsync(validFilter.ID);
            var data = entity.MapItemFieldsByMapToAnnotation<Location>();

            // response wrapper
            return data;
        }
    }
}