﻿using AutoMapper;
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

namespace bbxBE.Application.Queries.qInvoice
{
    public class QueryInvoice : QueryParameter, IRequest<PagedResponse<IEnumerable<Entity>>>
    {

        [ColumnLabel("Raktár")]
        [Description("Raktár")]
        public string WarehouseCode { get; set; }

        [ColumnLabel("Bizonylatszám")]
        [Description("Bizonylat sorszáma")]
        public string InvoiceNumber { get; set; }

        [ColumnLabel("Bizonylattípus")]
        [Description("Bizonylattípus")]
        public string InvoiceType { get; set; }


        [ColumnLabel("Kelt.tól")]
        [Description("Kiállítás dátumától")]
        public DateTime? InvoiceIssueDateFrom { get; set; }

        [ColumnLabel("Kelt.ig")]
        [Description("Kiállítás dátumáig")]
        public DateTime? InvoiceIssueDateTo { get; set; }

        [ColumnLabel("Teljesítés tól")]
        [Description("Teljesítés dátumától")]
        public DateTime? InvoiceDeliveryDateFrom { get; set; }

        [ColumnLabel("Teljesítés ig")]
        [Description("Teljesítés dátumig")]
        public DateTime? InvoiceDeliveryDateTo { get; set; }

        //Teljes reációs szerkezet kell? I/N
        public bool FullData { get; set; } = true;

    }

    public class QueryInvoiceHandler : IRequestHandler<QueryInvoice, PagedResponse<IEnumerable<Entity>>>
    {
        private readonly IInvoiceRepositoryAsync _invoiceRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public QueryInvoiceHandler(IInvoiceRepositoryAsync invoiceRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<PagedResponse<IEnumerable<Entity>>> Handle(QueryInvoice request, CancellationToken cancellationToken)
        {

 
            var validFilter = request; 
            var pagination = request;

            // query based on filter
            var entities = await _invoiceRepository.QueryPagedInvoiceAsync(validFilter);
            var data = entities.data.MapItemsFieldsByMapToAnnotation<GetInvoiceViewModel>();
            RecordsCount recordCount = entities.recordsCount;

            // response wrapper
            return new PagedResponse<IEnumerable<Entity>>(data, validFilter.PageNumber, validFilter.PageSize, recordCount);
        }
    }
}