using bbxBE.Common.Consts;
using bxBE.Application.Commands.cmdLocation;
using FluentValidation;

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
            }
        }
    }
}