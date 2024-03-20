using AutoMapper;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Queries.qInvoice
{
    public class QueryCustomerInvoiceSummary : QueryParameter, IRequest<PagedResponse<IList<GetCustomerInvoiceSummary>>>
    {
        [ColumnLabel("B/K")]
        [Description("Bejővő/Kimenő")]
        public bool Incoming { get; set; }

        [ColumnLabel("Ügyfél")]
        [Description("Ügyfél ID")]
        public long? CustomerID { get; set; }

        [ColumnLabel("Raktár")]
        [Description("Raktár")]
        public string WarehouseCode { get; set; }

        [ColumnLabel("Teljesítés tól")]
        [Description("Teljesítés dátumától")]
        public DateTime? InvoiceDeliveryDateFrom { get; set; }

        [ColumnLabel("Teljesítés ig")]
        [Description("Teljesítés dátumig")]
        public DateTime? InvoiceDeliveryDateTo { get; set; }
    }

    public class QueryCustomerInvoiceSummaryHandler : IRequestHandler<QueryCustomerInvoiceSummary, PagedResponse<IList<GetCustomerInvoiceSummary>>>
    {
        private readonly IInvoiceRepositoryAsync _invoiceRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public QueryCustomerInvoiceSummaryHandler(IInvoiceRepositoryAsync invoiceRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<PagedResponse<IList<GetCustomerInvoiceSummary>>> Handle(QueryCustomerInvoiceSummary request, CancellationToken cancellationToken)
        {

            // query based on filter
            var result = await _invoiceRepository.QueryPagedCustomerInvoiceSummaryAsync(request);
            RecordsCount recordCount = result.recordsCount;
            // response wrapper
            return new PagedResponse<IList<GetCustomerInvoiceSummary>>(result.data, request.PageNumber, request.PageSize, recordCount,
                        result.sumInvoiceNetAmountHUF, result.sumInvoiceGrossAmountHUF - result.sumInvoiceNetAmountHUF, result.sumInvoiceGrossAmountHUF);
        }
    }
}