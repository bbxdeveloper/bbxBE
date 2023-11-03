using bbxBE.Common.Consts;
using FluentValidation;

namespace bbxBE.Application.Commands.cmdNAV
{

    public class manageAnnulmentNAVValidator : AbstractValidator<manageAnnulmentNAVCommand>
    {

        public manageAnnulmentNAVValidator()
        {
            RuleFor(p => p.InvoiceNumber)
                   .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);

        }

    }
}