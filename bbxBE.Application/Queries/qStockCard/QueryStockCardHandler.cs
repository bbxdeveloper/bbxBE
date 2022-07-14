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

namespace bbxBE.Application.Queries.qStockCard
{
    public class QueryStockCard : QueryParameter, IRequest<PagedResponse<IEnumerable<Entity>>>
    {


        [ColumnLabel("Raktár")]
        [Description("Raktár")]
        public long? WarehouseID { get; set; }

        [ColumnLabel("Termék")]
        [Description("Termék")]
        public long? ProductID { get; set; }

        [ColumnLabel("Dátumtól")]
        [Description("Dátumtól")]
        public DateTime? StockCardDateFrom { get; set; }

        [ColumnLabel("Dátumig")]
        [Description("Dátumig")]
        public DateTime? StockCardDateTo { get; set; }


        [ColumnLabel("Kapcsolt bizonylat")]
        [Description("Kapcsolt bizonylat")]
        public string XRel { get; set; }



    }

    public class QueryStockCardHandler : IRequestHandler<QueryStockCard, PagedResponse<IEnumerable<Entity>>>
    {
        private readonly IStockCardRepositoryAsync _StockCardRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public QueryStockCardHandler(IStockCardRepositoryAsync StockCardRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _StockCardRepository = StockCardRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<PagedResponse<IEnumerable<Entity>>> Handle(QueryStockCard request, CancellationToken cancellationToken)
        {
            var validFilter = request;
            var pagination = request;

            /* TODO: törölni
            //filtered fields security
            if (!string.IsNullOrEmpty(validFilter.Fields))
            {
                //limit to fields in view model
                validFilter.Fields = _modelHelper.ValidateModelFields<GetStockCardViewModel, StockCard>(validFilter.Fields);
            }
  
            if (string.IsNullOrEmpty(validFilter.Fields))
            {
                //default fields from view model
                validFilter.Fields = _modelHelper.GetQueryableFields<GetStockCardViewModel, StockCard>();
            }
            */


            // query based on filter
            var entities = await _StockCardRepository.QueryPagedStockCardAsync(validFilter);
            var data = entities.data.MapItemsFieldsByMapToAnnotation<GetStockCardViewModel>();
            RecordsCount recordCount = entities.recordsCount;

            // response wrapper
            return new PagedResponse<IEnumerable<Entity>>(data, validFilter.PageNumber, validFilter.PageSize, recordCount);
        }
    }
}