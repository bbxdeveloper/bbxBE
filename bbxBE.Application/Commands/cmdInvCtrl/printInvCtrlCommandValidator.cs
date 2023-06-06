using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common.Consts;
using FluentValidation;

namespace bbxBE.Application.Commands.cmdInvCtrl
{

    public class printInvCtrlCommandValidator : AbstractValidator<PrintInvCtrlCommand>
    {
        private readonly IInvCtrlRepositoryAsync _invCtrlRepository;

        public printInvCtrlCommandValidator(IInvCtrlRepositoryAsync invCtrlRepository)
        {
            _invCtrlRepository = invCtrlRepository;
            RuleFor(p => p.InvCtrlPeriodID)
                .GreaterThan(0).WithMessage(bbxBEConsts.ERR_GREATGHERTHANZERO)
                .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED);
            RuleFor(p => p.InvPeriodTitle)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);
            RuleFor(p => p.IsInStock)
                .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED);
        }
    }
}
