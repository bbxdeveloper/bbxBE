﻿using bbxBE.Application.Consts;
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

    public class createInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
    {
        private readonly IInvoiceRepositoryAsync _InvoiceRepository;

        public createInvoiceCommandValidator(IInvoiceRepositoryAsync InvoiceRepository)
        {
            this._InvoiceRepository = InvoiceRepository;

            RuleFor(r => r.WarehouseCode)
                .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED);

            RuleFor(r => r.InvoiceIssueDate)
                .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED);
            
            RuleFor(r => r.InvoiceDeliveryDate)
                .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED);

            RuleFor(r => r.PaymentDate)
                .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED);

            RuleFor(r => new { r.InvoiceIssueDate, r.InvoiceDeliveryDate }).Must(m => m.InvoiceIssueDate >= m.InvoiceDeliveryDate)
                .WithMessage(bbxBEConsts.INV_DATE1);
            RuleFor(r => new { r.InvoiceIssueDate, r.PaymentDate }).Must(m => m.InvoiceIssueDate <= m.PaymentDate)
                .WithMessage(bbxBEConsts.INV_DATE2);

            RuleFor(r => r.CustomerID)
                .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED);

            RuleFor(r => r.PaymentMethod)
                .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED)
                .Must(CheckPaymentMethod).WithMessage(bbxBEConsts.FV_INVPAYMENTMETHOD);


            RuleFor(r => r.CurrencyCode)
                .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED)
                .Must(CheckCurrency).WithMessage(bbxBEConsts.FV_INVCURRENCY);


            RuleFor(r => r.ExchangeRate)
              .GreaterThan(0).WithMessage(bbxBEConsts.FV_EXCHANGERATE);


            RuleFor(p => new { p.InvoiceLines }).Must(m => m.InvoiceLines.Count > 0)
                .WithMessage(bbxBEConsts.INV_LINES);

           //invoiceline-ekre is validálást!!
           /*
            RuleFor(p => p.UnitOfMeasure)
                 .MustAsync(CheckUnitOfMEasureAsync).WithMessage(bbxBEConsts.FV_INVUNITOFMEASURE);
           */

        }

        private async Task<bool> IsUniqueInvoiceNumberAsync(string InvoiceCode, CancellationToken cancellationToken)
        {
            if (InvoiceCode.Length != 0)
            {
                return await _InvoiceRepository.IsUniqueInvoiceNumberAsync(InvoiceCode);
            }
            else
            {
                return true;
            }

        }
        private bool CheckPaymentMethod(string PaymentMethod)
        {
            var valid = Enum.TryParse(PaymentMethod, out PaymentMethodType pm);
            return valid;
        }

        private bool CheckCurrency(string Currency)
        {
            var valid = Enum.TryParse(Currency, out enCurrencyCodes curr);
            return valid;
        }

    }
}