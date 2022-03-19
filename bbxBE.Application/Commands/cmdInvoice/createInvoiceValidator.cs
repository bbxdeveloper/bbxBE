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
            RuleFor(p => p.InvoiceCode)
                 .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                 .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED)
                    .MustAsync(
                         async (model, Name, cancellation) =>
                         {
                             return await IsUniqueInvoiceNumberAsync(Name, cancellation);
                         }
                     ).WithMessage(bbxBEConsts.FV_EXISTS)
                 .MaximumLength(bbxBEConsts.CodeLen).WithMessage(bbxBEConsts.FV_MAXLEN);

            RuleFor(p => p.InvoiceDescription)
                .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED)
                .MaximumLength(bbxBEConsts.DescriptionLen).WithMessage(bbxBEConsts.FV_MAXLEN);

            RuleFor(p => p.Prefix)
                .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED)
                .MaximumLength(bbxBEConsts.CodeLen).WithMessage(bbxBEConsts.FV_MAXLEN);

            RuleFor(p => p.NumbepartLength)
                .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED)
                .InclusiveBetween(1, 10).WithMessage(bbxBEConsts.FV_RANGE);
        }
        /*
                    .WithColumn("NumbepartLength").AsInt64().NotNullable()
                    .WithColumn("Suffix").AsString().NotNullable();

        */
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