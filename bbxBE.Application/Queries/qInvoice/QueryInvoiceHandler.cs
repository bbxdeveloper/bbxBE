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


        [ColumnLabel("Keltezés től")]
        [Description("Kiállítás dátumától")]
        public DateTime? InvoiceIssueDateFrom { get; set; }

        [ColumnLabel("Keltezés ig")]
        [Description("Kiállítás dátumáig")]
        public DateTime? InvoiceIssueDateTo { get; set; }

        [ColumnLabel("Teljesítés tól")]
        [Description("Teljesítés dátumától")]
        public DateTime? InvoiceDeliveryDateFrom { get; set; }

        [ColumnLabel("Teljesítés ig")]
        [Description("Teljesítés dátumig")]
        public DateTime? InvoiceDeliveryDateTo { get; set; }

        [ColumnLabel("Partner azonosító")]
        [Description("Partner azonosító")]
        public long? CustomerID { get; set; }

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
            // query based on filter
            var result = await _invoiceRepository.QueryPagedInvoiceAsync(request);
            var data = result.data.MapItemsFieldsByMapToAnnotation<GetInvoiceViewModel>();
            RecordsCount recordCount = result.recordsCount;

            // response wrapper
            return new PagedResponse<IEnumerable<Entity>>(data, request.PageNumber, request.PageSize, recordCount,
                result.sumInvoiceNetAmountHUF, result.sumInvoiceGrossAmountHUF - result.sumInvoiceNetAmountHUF, result.sumInvoiceGrossAmountHUF);
        }
    }
}