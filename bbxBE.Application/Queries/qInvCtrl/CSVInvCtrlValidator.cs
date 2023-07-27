using bbxBE.Common.Consts;
using FluentValidation;

namespace bbxBE.Application.Queries.qInvCtrl
{

    public class CSVInvCtrlValidator : AbstractValidator<CSVInvCtrl>
    {

        public CSVInvCtrlValidator()
        {
            RuleFor(p => p.InvCtrlPeriodID)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);
        }

    }
}