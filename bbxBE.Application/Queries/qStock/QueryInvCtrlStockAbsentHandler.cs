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
using bbxBE.Common.Attributes;
using System.ComponentModel;
using System;

namespace bbxBE.Application.Queries.qStock
{
    public class QueryInvCtrlStockAbsent : QueryParameter, IRequest<PagedResponse<IEnumerable<Entity>>>
    {

        public long InvCtrlPeriodID { get; set; }
        public bool IsInStock { get; set; }

        public string SearchString { get; set; }

    }

    public class QueryInvCtrlStockAbsentHandler : IRequestHandler<QueryInvCtrlStockAbsent, PagedResponse<IEnumerable<Entity>>>
    {
        private readonly IStockRepositoryAsync _stockRepository;
        private readonly IInvCtrlRepositoryAsync _invCtrlRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public QueryInvCtrlStockAbsentHandler(IStockRepositoryAsync stockRepository,
                IInvCtrlRepositoryAsync invCtrlRepository,
                IMapper mapper, IModelHelper modelHelper)
        {
            _stockRepository = stockRepository;
            _invCtrlRepository = invCtrlRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<PagedResponse<IEnumerable<Entity>>> Handle(QueryInvCtrlStockAbsent request, CancellationToken cancellationToken)
        {
            var validFilter = request;
            var pagination = request;


            // query based on filter
            var entities = await _invCtrlRepository.QueryInvCtrlStockAbsentAsync(validFilter);
            var data = entities.data.MapItemsFieldsByMapToAnnotation<GetStockViewModel>();
            RecordsCount recordCount = entities.recordsCount;

            // response wrapper
            return new PagedResponse<IEnumerable<Entity>>(data, validFilter.PageNumber, validFilter.PageSize, recordCount);
        }
    }
}