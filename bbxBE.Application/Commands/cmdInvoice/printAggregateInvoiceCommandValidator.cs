﻿using bbxBE.Application.Commands.cmdImport;
using bbxBE.Common.Consts;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
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

    public class PrintAggregateInvoiceCommandValidator : AbstractValidator<PrintAggregateInvoiceCommand>
    {
        private readonly IInvoiceRepositoryAsync _invoiceRepository;

        public PrintAggregateInvoiceCommandValidator(IInvoiceRepositoryAsync invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
            RuleFor(p => p.ID)
                .GreaterThan(0).WithMessage(bbxBEConsts.ERR_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED);

        }
    }
}