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
                 .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
               .Must(CheckInvoiceType)
                .WithMessage((model, field) => string.Format(bbxBEConsts.ERR_INVOICETYPE, model.InvoiceType));

            RuleFor(f => f.InvoiceDeliveryDateFrom)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                .LessThanOrEqualTo(f => f.InvoiceDeliveryDateTo.Value).WithMessage(bbxBEConsts.ERR_DATEINTERVAL)
                .When(f => f.InvoiceDeliveryDateTo.HasValue);

            RuleFor(f => f.InvoiceIssueDateFrom)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                .LessThanOrEqualTo(f => f.InvoiceIssueDateTo.Value).WithMessage(bbxBEConsts.ERR_DATEINTERVAL)
                .When(f => f.InvoiceIssueDateTo.HasValue);

        }

        public bool CheckInvoiceType(string invoiceType)
        {
            if (string.IsNullOrWhiteSpace(invoiceType)) { return false; }
            var valid = Enum.TryParse(invoiceType, out enInvoiceType it);
            return valid;
        }
    }

}