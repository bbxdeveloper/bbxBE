using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using FluentValidation;
using System;

namespace bbxBE.Application.Queries.qInvoice
{

    public class QueryUnpaidInvoiceValidator : AbstractValidator<QueryUnpaidInvoice>
    {

        public QueryUnpaidInvoiceValidator()
        {
            RuleFor(p => p.Incoming)
                 .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED);


            RuleFor(f => f.InvoiceDeliveryDateFrom)
                .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED)
                .LessThanOrEqualTo(f => f.InvoiceDeliveryDateTo.Value).WithMessage(bbxBEConsts.ERR_DATEINTERVAL)
                .When(f => f.InvoiceDeliveryDateTo.HasValue);

            RuleFor(f => f.InvoiceIssueDateFrom)
                 .LessThanOrEqualTo(f => f.InvoiceIssueDateTo.Value).WithMessage(bbxBEConsts.ERR_DATEINTERVAL)
                 .When(f => f.InvoiceIssueDateFrom.HasValue && f.InvoiceIssueDateTo.HasValue);

            RuleFor(f => f.PaymentDateFrom)
                 .LessThanOrEqualTo(f => f.PaymentDateTo.Value).WithMessage(bbxBEConsts.ERR_DATEINTERVAL)
                 .When(f => f.PaymentDateFrom.HasValue && f.PaymentDateTo.HasValue);
        }

        public bool CheckInvoiceType(string invoiceType)
        {
            if (string.IsNullOrWhiteSpace(invoiceType)) { return false; }
            var valid = Enum.TryParse(invoiceType, out enInvoiceType it);
            return valid;
        }
    }

}