using bbxBE.Application.Consts;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
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

            RuleFor(p => p.WarehouseCode)
                .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED);

            RuleFor(p => p.InvoiceIssueDate)
                .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED);
            
            RuleFor(p => p.InvoiceDeliveryDate)
                .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED);

            RuleFor(p => p.PaymentDate)
                .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED);

            RuleFor(p => new { p.InvoiceIssueDate, p.InvoiceDeliveryDate }).Must(m => m.InvoiceIssueDate >= m.InvoiceDeliveryDate)
                .WithMessage(bbxBEConsts.INV_DATE1);
            RuleFor(p => new { p.InvoiceIssueDate, p.PaymentDate }).Must(m => m.InvoiceIssueDate <= m.PaymentDate)
                .WithMessage(bbxBEConsts.INV_DATE2);

            RuleFor(p => p.CustomerID)
                .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED);

            RuleFor(p => p.PaymentMethod)
                .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED);

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

    }
}