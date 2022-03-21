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

    public class createInvoiceCommandValidator : AbstractValidator<CreateIncomingInvoiceCommand>
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
itt tartok
            RuleFor(p => new { p.InvoiceIssueDate, p.InvoiceDeliveryDate }).Must(x => ValidZipCounty(x.Zip, x.CountyId))
                                      .WithMessage("Wrong Zip County");

            [ColumnLabel("Kelt")]
            [Description("Kiállítás dátuma")]
            public DateTime InvoiceIssueDate { get; set; }

            [ColumnLabel("Teljesítés")]
            [Description("Teljesítés dátuma")]
            public DateTime InvoiceDeliveryDate { get; set; }

            [ColumnLabel("Fiz.hat")]
            [Description("Fizetési határidő dátuma")]
            public DateTime PaymentDate { get; set; }


            [ColumnLabel("Ügyfél ID")]
            [Description("Ügyfél ID")]
            public long CustomerID { get; set; }

            [ColumnLabel("Bevétel biz.")]
            [Description("Bevétel alapjául szolgáló bizonylat")]
            public string IncomingInvReference { get; set; }

            [ColumnLabel("Fiz.mód")]
            [Description("Fizetési mód")]
            public string PaymentMethod { get; set; }

            [ColumnLabel("Megjegyzés")]
            [Description("Megjegyzés")]
            public string Notice { get; set; }  //AdditionalInvoiceData-ban tároljuk!

            [ColumnLabel("Nettó")]
            [Description("A számla nettó összege a számla pénznemében")]
            public decimal InvoiceNetAmount { get; set; }

            [ColumnLabel("Áfa")]
            [Description("A számla áfa összege a számla pénznemében")]
            public decimal InvoiceVatAmount { get; set; }
            [ColumnLabel("Bruttó")]
            [Description("A számla végösszege a számla pénznemében")]
            public decimal InvoiceGrossAmount { get; set; }

            [ColumnLabel("Számlasorok")]
            [Description("Számlasorok")]
            public List<InvoiceLine> InvoiceLines { get; set; } = new List<InvoiceLine>();

            [ColumnLabel("Áfaösszesítők")]
            [Description("Áfaösszesítők")]
            public List<SummaryByVatRate> SummaryByVatRates { get; set; } = new List<SummaryByVatRate>();





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