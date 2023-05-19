using bbxBE.Common.Consts;
using FluentValidation;

namespace bbxBE.Application.Commands.cmdWhsTransfer
{
    public class printWhsTransferCommandValidator : AbstractValidator<PrintWhsTransferCommand>
    {
        public printWhsTransferCommandValidator()
        {
            RuleFor(p => p.ID)
                .GreaterThan(0).WithMessage(bbxBEConsts.ERR_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED);

        }
    }
}
