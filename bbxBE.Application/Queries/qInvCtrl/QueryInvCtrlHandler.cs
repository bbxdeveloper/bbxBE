using AutoMapper;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
using bbxBE.Domain.Entities;
using bbxBE.Domain.Extensions;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Queries.qInvCtrl
{
    public class QueryInvCtrl : QueryParameter, IRequest<PagedResponse<IEnumerable<Entity>>>
    {
        [ColumnLabel("Leltáridőszak ID")]
        [Description("Leltáridőszak ID")]
        public long? InvCtrlPeriodID { get; set; }          //Leltáridőszaki leltár esetén értlemezett

        [ColumnLabel("Kezdődátum")]
        [Description("Kezdődátum")]
        public DateTime? DateFrom { get; set; }             //Folyamatos leltár esetén értelmezett

        [ColumnLabel("Végdátum")]
        [Description("Végdátum")]
        public DateTime? DateTo { get; set; }               //Folyamatos leltár esetén értelmezett

        [ColumnLabel("Keresett adat")]
        [Description("Keresett adat")]
        public string SearchString { get; set; }

        [ColumnLabel("Hiány/többlet")]
        [Description("Hiány/többlet lekérdezése")]
        public bool? ShowDeficit { get; set; }              //null=minden adat, True=leltárhiány, False=leltártöbblet
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
            // query based on filter
            var entities = await _InvCtrlRepository.QueryPagedInvCtrlAsync(request);
            var data = entities.data.MapItemsFieldsByMapToAnnotation<GetInvCtrlViewModel>();
            RecordsCount recordCount = entities.recordsCount;

            // response wrapper
            return new PagedResponse<IEnumerable<Entity>>(data, request.PageNumber, request.PageSize, recordCount);
        }
    }
}