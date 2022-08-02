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

namespace bbxBE.Application.Queries.qInvCtrlPeriod
{
    public class QueryInvCtrlPeriod : QueryParameter, IRequest<PagedResponse<IEnumerable<Entity>>>
    {
    }

    public class QueryInvCtrlPeriodHandler : IRequestHandler<QueryInvCtrlPeriod, PagedResponse<IEnumerable<Entity>>>
    {
        private readonly IInvCtrlPeriodRepositoryAsync _InvCtrlPeriodRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public QueryInvCtrlPeriodHandler(IInvCtrlPeriodRepositoryAsync InvCtrlPeriodRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _InvCtrlPeriodRepository = InvCtrlPeriodRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<PagedResponse<IEnumerable<Entity>>> Handle(QueryInvCtrlPeriod request, CancellationToken cancellationToken)
        {
            var validFilter = request;
            var pagination = request;
            


            // query based on filter
            var entities = await _InvCtrlPeriodRepository.QueryPagedInvCtrlPeriodAsync(validFilter);
            var data = entities.data.MapItemsFieldsByMapToAnnotation<GetInvCtrlPeriodViewModel>();
            RecordsCount recordCount = entities.recordsCount;

            // response wrapper
            return new PagedResponse<IEnumerable<Entity>>(data, validFilter.PageNumber, validFilter.PageSize, recordCount);
        }
    }
}