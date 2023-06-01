using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common.Consts;
using FluentValidation;

namespace bbxBE.Application.Commands.cmdInvCtrlPeriod
{

    public class printInvCtrlPeriodCommandValidator : AbstractValidator<PrintInvCtrlPeriodCommand>
    {
        private readonly IInvCtrlPeriodRepositoryAsync _invCtrlPeriodRepository;

        public printInvCtrlPeriodCommandValidator(IInvCtrlPeriodRepositoryAsync invCtrlPeriodRepository)
        {
            _invCtrlPeriodRepository = invCtrlPeriodRepository;
            RuleFor(p => p.InvCtrlPeriodID)
                .GreaterThan(0).WithMessage(bbxBEConsts.ERR_GREATGHERTHANZERO)
                .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED);
            RuleFor(p => p.InvPeriodTitle)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);
        }
    }
}
