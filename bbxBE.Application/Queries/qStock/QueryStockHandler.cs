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
    public class QueryStock : QueryParameter, IRequest<PagedResponse<IEnumerable<Entity>>>
    {

        [ColumnLabel("Raktár ID")]
        [Description("Raktár ID")]
        public long WarehouseID { get; set; }

        [ColumnLabel("Termék ID")]
        [Description("Termék ID")]
        public long ProductID { get; set; }
    }

    public class QueryStockHandler : IRequestHandler<QueryStock, PagedResponse<IEnumerable<Entity>>>
    {
        private readonly IStockRepositoryAsync _StockRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public QueryStockHandler(IStockRepositoryAsync StockRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _StockRepository = StockRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<PagedResponse<IEnumerable<Entity>>> Handle(QueryStock request, CancellationToken cancellationToken)
        {
            var validFilter = request;
            var pagination = request;

            /* TODO: törölni
            //filtered fields security
            if (!string.IsNullOrEmpty(validFilter.Fields))
            {
                //limit to fields in view model
                validFilter.Fields = _modelHelper.ValidateModelFields<GetStockViewModel, Stock>(validFilter.Fields);
            }
  
            if (string.IsNullOrEmpty(validFilter.Fields))
            {
                //default fields from view model
                validFilter.Fields = _modelHelper.GetQueryableFields<GetStockViewModel, Stock>();
            }
            */


            // query based on filter
            var entities = await _StockRepository.QueryPagedStockAsync(validFilter);
            var data = entities.data.MapItemsFieldsByMapToAnnotation<GetStockViewModel>();
            RecordsCount recordCount = entities.recordsCount;

            // response wrapper
            return new PagedResponse<IEnumerable<Entity>>(data, validFilter.PageNumber, validFilter.PageSize, recordCount);
        }
    }
}