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

namespace bbxBE.Application.Queries.qCounter
{
    public class GetCounter:  IRequest<Entity>
    {
        public long ID { get; set; }
  //      public string Fields { get; set; }
    }

    public class GetCounterHandler : IRequestHandler<GetCounter, Entity>
    {
        private readonly ICounterRepositoryAsync _counterRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public GetCounterHandler(ICounterRepositoryAsync counterRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _counterRepository = counterRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<Entity> Handle(GetCounter request, CancellationToken cancellationToken)
        {
            var entity = await _counterRepository.GetCounterAsync(request);
            var data = entity.MapItemFieldsByMapToAnnotation<GetCounterViewModel>();

            // response wrapper
            return data;
        }
    }
}