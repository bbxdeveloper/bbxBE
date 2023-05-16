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

namespace bbxBE.Application.Queries.qWhsTransfer
{
    public class QueryWhsTransfer : QueryParameter, IRequest<PagedResponse<IEnumerable<Entity>>>
    {


        [ColumnLabel("Státusz")]
        [Description("Státusz")]
        public string WhsTransferStatus { get; set; }

        [ColumnLabel("Kiadás raktárkód")]
        [Description("Kiadás raktárkód")]
        public string FromWarehouseCode { get; set; }

        [ColumnLabel("Bevétel raktárkód")]
        [Description("Bevétel raktárkód")]
        public string ToWarehouseCode { get; set; }

        [ColumnLabel("Kezdődátum")]
        [Description("Kezdődátum")]
        public DateTime? TransferDateFrom { get; set; }

        [ColumnLabel("Végdátum")]
        [Description("Végdátum")]
        public DateTime? TransferDateTo { get; set; }
    }

    public class QueryWhsTransferHandler : IRequestHandler<QueryWhsTransfer, PagedResponse<IEnumerable<Entity>>>
    {
        private readonly IWhsTransferRepositoryAsync _WhsTransferRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public QueryWhsTransferHandler(IWhsTransferRepositoryAsync WhsTransferRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _WhsTransferRepository = WhsTransferRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<PagedResponse<IEnumerable<Entity>>> Handle(QueryWhsTransfer request, CancellationToken cancellationToken)
        {
            var validFilter = request;
            var pagination = request;

            // query based on filter
            var entities = await _WhsTransferRepository.QueryPagedWhsTransferAsync(validFilter);
            var data = entities.data.MapItemsFieldsByMapToAnnotation<GetWhsTransferViewModel>();
            RecordsCount recordCount = entities.recordsCount;

            // response wrapper
            return new PagedResponse<IEnumerable<Entity>>(data, validFilter.PageNumber, validFilter.PageSize, recordCount);
        }
    }
}