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

namespace bbxBE.Application.Queries.qVatRate
{
    public class GetVatRate:  IRequest<Entity>
    {
        public long ID { get; set; }
  //      public string Fields { get; set; }
    }

    public class GetVatRateHandler : IRequestHandler<GetVatRate, Entity>
    {
        private readonly IVatRateRepositoryAsync _vatRateRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public GetVatRateHandler(IVatRateRepositoryAsync vatRateRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _vatRateRepository = vatRateRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<Entity> Handle(GetVatRate request, CancellationToken cancellationToken)
        {
            var entity = await _vatRateRepository.GetVatRateAsync(request.ID);
            var data = entity.MapItemFieldsByMapToAnnotation<GetVatRateViewModel>();

            // response wrapper
            return data;
        }
    }
}