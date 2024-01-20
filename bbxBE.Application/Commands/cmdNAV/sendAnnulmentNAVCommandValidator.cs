using bbxBE.Common.Consts;
using FluentValidation;

namespace bbxBE.Application.Commands.cmdNAV
{

    public class sendAnnulmentNAVCommandValidator : AbstractValidator<sendAnnulmentNAVCommand>
    {

        public sendAnnulmentNAVCommandValidator()
        {
            RuleFor(p => p.InvoiceNumber)
                   .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);

        }

    }
}