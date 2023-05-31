using bbxBE.Common.Consts;
using FluentValidation;

namespace bbxBE.Application.Queries.qInvoice
{
    public class QueryCustomerInvoiceSummaryValidator : AbstractValidator<QueryCustomerInvoiceSummary>
    {
        public QueryCustomerInvoiceSummaryValidator()
        {
            RuleFor(f => f.InvoiceDeliveryDateFrom)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                .LessThanOrEqualTo(f => f.InvoiceDeliveryDateTo.Value).WithMessage(bbxBEConsts.ERR_DATEINTERVAL)
                .When(f => f.InvoiceDeliveryDateTo.HasValue);


        }
    }
}