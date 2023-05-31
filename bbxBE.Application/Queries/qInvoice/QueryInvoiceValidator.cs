using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using FluentValidation;
using System;

namespace bbxBE.Application.Queries.qInvoice
{

    public class QueryInvoiceValidatorValidator : AbstractValidator<QueryInvoice>
    {

        public QueryInvoiceValidatorValidator()
        {
            RuleFor(p => p.InvoiceType)
                .Must(CheckInvoiceType)
                .WithMessage((model, field) => string.Format(bbxBEConsts.ERR_INVOICETYPE));

            RuleFor(f => f.InvoiceDeliveryDateTo)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                .LessThanOrEqualTo(f => f.InvoiceDeliveryDateFrom.Value).WithMessage(bbxBEConsts.ERR_DATEINTERVAL)
                .When(f => f.InvoiceDeliveryDateFrom.HasValue);

            RuleFor(f => f.InvoiceIssueDateTo)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                .LessThanOrEqualTo(f => f.InvoiceIssueDateFrom.Value).WithMessage(bbxBEConsts.ERR_DATEINTERVAL)
                .When(f => f.InvoiceIssueDateFrom.HasValue);

        }

        public bool CheckInvoiceType(string invoiceType)
        {
            var valid = Enum.TryParse(invoiceType, out enInvoiceType it);
            return valid;
        }
    }

}