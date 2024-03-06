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

namespace bbxBE.Application.Queries.qInvoice
{
    public class QueryXChange : QueryParameter, IRequest<PagedResponse<IEnumerable<Entity>>>
    {


        [ColumnLabel("Dátumtól")]
        [Description("Dátumtól")]
        public DateTime CreateTimeFrom { get; set; }

        [ColumnLabel("Dátumtig")]
        [Description("Dátumtig")]
        public DateTime? CreateTimeTo { get; set; }

        [ColumnLabel("Bizonylatszám")]
        [Description("Bizonylat sorszáma")]
        public string InvoiceNumber { get; set; }

        [ColumnLabel("Figyelmeztetés nézet")]
        [Description("Figyelmeztetés nézet")]
        public bool WarningView { get; set; } = false;

        [ColumnLabel("Hiba nézet")]
        [Description("Hiba nézet")]
        public bool ErrorView { get; set; } = false;
    }

    public class QueryXChangeHandler : IRequestHandler<QueryXChange, PagedResponse<IEnumerable<Entity>>>
    {
        private readonly INAVXChangeRepositoryAsync _navXChangeRepository;
        private readonly IWarehouseRepositoryAsync _warehouseRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public QueryXChangeHandler(INAVXChangeRepositoryAsync navXChangeRepository,
                                   IWarehouseRepositoryAsync warehouseRepository,
                                    IMapper mapper, IModelHelper modelHelper)
        {
            _navXChangeRepository = navXChangeRepository;
            _warehouseRepository = warehouseRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<PagedResponse<IEnumerable<Entity>>> Handle(QueryXChange request, CancellationToken cancellationToken)
        {


            // query based on filter
            var entities = await _navXChangeRepository.QueryPagedNAVXChangeAsync(request);
            var data = entities.data.MapItemsFieldsByMapToAnnotation<GetInvoiceViewModel>();
            RecordsCount recordCount = entities.recordsCount;

            // response wrapper
            return new PagedResponse<IEnumerable<Entity>>(data, request.PageNumber, request.PageSize, recordCount);
        }
    }
}