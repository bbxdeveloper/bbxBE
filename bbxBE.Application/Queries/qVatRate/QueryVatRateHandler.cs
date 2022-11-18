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
    public class QueryVatRate : QueryParameter, IRequest<PagedResponse<IEnumerable<Entity>>>
    {
        public string SearchString { get; set; }
    }

    public class QueryVatRateHandler : IRequestHandler<QueryVatRate, PagedResponse<IEnumerable<Entity>>>
    {
        private readonly IVatRateRepositoryAsync _VatRateRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public QueryVatRateHandler(IVatRateRepositoryAsync VatRateRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _VatRateRepository = VatRateRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<PagedResponse<IEnumerable<Entity>>> Handle(QueryVatRate request, CancellationToken cancellationToken)
        {
            var validFilter = request;
            
            // query based on filter
            var entities = await _VatRateRepository.QueryPagedVatRate(validFilter);
            var data =   entities.data.MapItemsFieldsByMapToAnnotation<GetVatRateViewModel>();
            RecordsCount recordCount = entities.recordsCount;

            // response wrapper
            return new PagedResponse<IEnumerable<Entity>>(data, validFilter.PageNumber, validFilter.PageSize, recordCount);
        }
    }
}