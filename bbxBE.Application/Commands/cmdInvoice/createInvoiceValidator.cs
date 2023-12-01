using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bxBE.Application.Commands.cmdInvoice;
using FluentValidation;
using System;
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
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);

            RuleFor(r => r.InvoiceIssueDate)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);

            RuleFor(r => r.InvoiceDeliveryDate)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);

            RuleFor(r => r.PaymentDate)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);

            RuleFor(r => r.CustomerInvoiceNumber)
                .NotEmpty()
                .When(p => p.Incoming)
                .WithMessage(bbxBEConsts.ERR_REQUIRED);

            RuleFor(r => new { r.InvoiceIssueDate, r.InvoiceDeliveryDate, r.Incoming }).Must(m => m.Incoming || m.InvoiceIssueDate >= m.InvoiceDeliveryDate)
                .WithMessage(bbxBEConsts.ERR_INV_DATE1);
            RuleFor(r => new { r.InvoiceIssueDate, r.PaymentDate, r.Incoming }).Must(m => m.Incoming || m.InvoiceIssueDate <= m.PaymentDate)
                .WithMessage(bbxBEConsts.ERR_INV_DATE2);

            RuleFor(r => r.CustomerID)
                .NotEmpty()
                .When(p => p.InvoiceType != enInvoiceType.BLK.ToString())
                .WithMessage(bbxBEConsts.ERR_REQUIRED);

            RuleFor(r => r.PaymentMethod)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                .Must(CheckPaymentMethod).WithMessage(bbxBEConsts.ERR_INVPAYMENTMETHOD);


            RuleFor(r => r.CurrencyCode)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                .Must(CheckCurrency).WithMessage(bbxBEConsts.ERR_INVCURRENCY);


            RuleFor(r => r.ExchangeRate)
              .GreaterThan(0).WithMessage(bbxBEConsts.ERR_EXCHANGERATE);


            RuleFor(p => new { p.InvoiceLines }).Must(m => m.InvoiceLines.Count > 0)
                .WithMessage(bbxBEConsts.ERR_INV_LINES);

            RuleForEach(r => r.InvoiceLines)
                 .SetValidator(model => new CreateInvoiceLinesCommandValidatror());



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



    public class CreateInvoiceLinesCommandValidatror : AbstractValidator<CreateInvoiceCommand.InvoiceLine>
    {
        public CreateInvoiceLinesCommandValidatror()
        {
            RuleFor(r => r.Quantity)
              .NotEmpty()
              .WithMessage(bbxBEConsts.ERR_REQUIRED);

            RuleFor(p => p.UnitOfMeasure)
                 .Must(CheckUnitOfMEasure).WithMessage((model, field) => string.Format(bbxBEConsts.ERR_INVUNITOFMEASURE2, model.LineNumber, model.ProductCode, model.UnitOfMeasure));
        }

        public bool CheckUnitOfMEasure(string unitOfMeasure)
        {
            var valid = Enum.TryParse(unitOfMeasure, out enUnitOfMeasure uom);
            return valid;
        }

    }

}