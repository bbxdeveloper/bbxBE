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

namespace bbxBE.Application.Queries.qInvCtrl
{
    public class QueryInvCtrl : QueryParameter, IRequest<PagedResponse<IEnumerable<Entity>>>
    {
        public long InvCtrlPeriodID { get; set; }
        public string SearchString { get; set; }
    }

    public class QueryInvCtrlHandler : IRequestHandler<QueryInvCtrl, PagedResponse<IEnumerable<Entity>>>
    {
        private readonly IInvCtrlRepositoryAsync _InvCtrlRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public QueryInvCtrlHandler(IInvCtrlRepositoryAsync InvCtrlRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _InvCtrlRepository = InvCtrlRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<PagedResponse<IEnumerable<Entity>>> Handle(QueryInvCtrl request, CancellationToken cancellationToken)
        {
            var validFilter = request;
            var pagination = request;
            

            // query based on filter
            var entities = await _InvCtrlRepository.QueryPagedInvCtrlAsync(validFilter);
            var data = entities.data.MapItemsFieldsByMapToAnnotation<GetInvCtrlViewModel>();
            RecordsCount recordCount = entities.recordsCount;

            // response wrapper
            return new PagedResponse<IEnumerable<Entity>>(data, validFilter.PageNumber, validFilter.PageSize, recordCount);
        }
    }
}