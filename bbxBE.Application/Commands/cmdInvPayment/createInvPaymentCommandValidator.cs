using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using bxBE.Application.Commands.cmdLocation;
using FluentValidation;
using System;

namespace bbxBE.Application.Commands.cmdLocation
{

    public class createInvPaymentCommandValidator : AbstractValidator<CreateInvPaymentCommand>
    {

        public createInvPaymentCommandValidator()
        {
            RuleForEach(r => r.InvPaymentItems)
                  .SetValidator(model => new CreateInvPaymentPaymentItemValidatror());

        }

        public class CreateInvPaymentPaymentItemValidatror : AbstractValidator<CreateInvPaymentCommand.InvPaymentItem>
        {
            public CreateInvPaymentPaymentItemValidatror()
            {
                RuleFor(r => r.InvoiceNumber)
                  .NotEmpty()
                  .WithMessage(bbxBEConsts.ERR_REQUIRED);

                RuleFor(r => r.BankTransaction)
                  .NotEmpty()
                  .WithMessage(bbxBEConsts.ERR_REQUIRED);


                RuleFor(r => r.CurrencyCode)
                    .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                    .Must(CheckCurrency).WithMessage(bbxBEConsts.ERR_INVCURRENCY);


            }
            private bool CheckCurrency(string Currency)
            {
                var valid = Enum.TryParse(Currency, out enCurrencyCodes curr);
                return valid;
            }
        }


    }
}