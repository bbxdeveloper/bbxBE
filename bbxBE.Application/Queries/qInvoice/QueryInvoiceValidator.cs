﻿using bbxBE.Common.Consts;
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

            RuleFor(f => f.InvoiceDeliveryDateTo)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                .GreaterThan(f => f.InvoiceDeliveryDateFrom.Value).WithMessage(bbxBEConsts.ERR_DATEINTERVAL)
                .When(f => f.InvoiceDeliveryDateFrom.HasValue);

            RuleFor(f => f.InvoiceIssueDateTo)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                .GreaterThan(f => f.InvoiceIssueDateFrom.Value).WithMessage(bbxBEConsts.ERR_DATEINTERVAL)
                .When(f => f.InvoiceIssueDateFrom.HasValue);

        }

        public bool CheckInvoiceType(string invoiceType)
        {
            if (string.IsNullOrWhiteSpace(invoiceType)) { return false; }
            var valid = Enum.TryParse(invoiceType, out enInvoiceType it);
            return valid;
        }
    }

}