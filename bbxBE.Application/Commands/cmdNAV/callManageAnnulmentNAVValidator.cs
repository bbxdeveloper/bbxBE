using bbxBE.Common.Consts;
using FluentValidation;

namespace bbxBE.Application.Commands.cmdNAV
{

    public class callManageAnnulmentNAVValidator : AbstractValidator<callManageAnnulmentNAVCommand>
    {

        public callManageAnnulmentNAVValidator()
        {
            RuleFor(p => p.InvoiceNumber)
                   .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);

        }

    }
}