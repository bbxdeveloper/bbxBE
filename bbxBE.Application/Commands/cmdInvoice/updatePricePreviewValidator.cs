using bbxBE.Common.Consts;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bxBE.Application.Commands.cmdInvoice;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Commands.cmdInvoice
{

    public class UpdatePricePreviewCommandValidator : AbstractValidator<UpdatePricePreviewCommand>
    {
        private readonly IInvoiceRepositoryAsync _InvoiceRepository;

        public UpdatePricePreviewCommandValidator(IInvoiceRepositoryAsync InvoiceRepository)
        {
            this._InvoiceRepository = InvoiceRepository;

            RuleFor(r => r.CustomerID)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);

            RuleFor(p => new { p.InvoiceLines }).Must(m => m.InvoiceLines.Count > 0)
                .WithMessage(bbxBEConsts.ERR_INV_LINES);

        }

    }
}