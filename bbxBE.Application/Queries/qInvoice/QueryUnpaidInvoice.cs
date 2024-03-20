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
    public class QueryUnpaidInvoice : QueryParameter, IRequest<PagedResponse<IEnumerable<Entity>>>
    {

        [ColumnLabel("Bizonylatszám")]
        [Description("Bizonylat sorszáma")]
        public string InvoiceNumber { get; set; }

        [ColumnLabel("Eredeti.biz")]
        [Description("Bevételhez eredeti bizonylata")]
        public string CustomerInvoiceNumber { get; set; }

        [ColumnLabel("Ügyfél")]
        [Description("Ügyfél ID")]
        public long? CustomerID { get; set; }

        [ColumnLabel("B/K")]
        [Description("Bejővő/Kimenő")]
        public bool Incoming { get; set; }

        [ColumnLabel("Teljesítés tól")]
        [Description("Teljesítés dátumától")]
        public DateTime InvoiceDeliveryDateFrom { get; set; }   //Kötelező

        [ColumnLabel("Teljesítés ig")]
        [Description("Teljesítés dátumig")]
        public DateTime? InvoiceDeliveryDateTo { get; set; }

        [ColumnLabel("Keltezés től")]
        [Description("Kiállítás dátumától")]
        public DateTime? InvoiceIssueDateFrom { get; set; }

        [ColumnLabel("Keltezés ig")]
        [Description("Kiállítás dátumáig")]
        public DateTime? InvoiceIssueDateTo { get; set; }


        [ColumnLabel("Fiz.hat.tól")]
        [Description("Fizetési határidő dátumától")]
        public DateTime? PaymentDateFrom { get; set; }

        [ColumnLabel("Fiz.hat.ig")]
        [Description("Fizetési határidő dátumáig")]
        public DateTime? PaymentDateTo { get; set; }

        [ColumnLabel("Lejárt tartozás?")]
        [Description("Lejárt tartozás?")]
        public bool Expired { get; set; } = false;

    }

    public class QueryUnpaidInvoiceHandler : IRequestHandler<QueryUnpaidInvoice, PagedResponse<IEnumerable<Entity>>>
    {
        private readonly IInvoiceRepositoryAsync _invoiceRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public QueryUnpaidInvoiceHandler(IInvoiceRepositoryAsync invoiceRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<PagedResponse<IEnumerable<Entity>>> Handle(QueryUnpaidInvoice request, CancellationToken cancellationToken)
        {

            var result = await _invoiceRepository.QueryPagedUnpaidInvoiceAsync(request);
            var data = result.data.MapItemsFieldsByMapToAnnotation<GetInvoiceViewModel>();
            RecordsCount recordCount = result.recordsCount;

            // response wrapper
            return new PagedResponse<IEnumerable<Entity>>(data, request.PageNumber, request.PageSize, recordCount,
                    result.sumInvoiceNetAmountHUF, result.sumInvoiceGrossAmountHUF - result.sumInvoiceNetAmountHUF, result.sumInvoiceGrossAmountHUF);
        }
    }
}