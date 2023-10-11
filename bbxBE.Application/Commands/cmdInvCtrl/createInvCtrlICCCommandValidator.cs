using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common.Consts;
using bxBE.Application.Commands.cmdInvCtrl;
using FluentValidation;

namespace bbxBE.Application.Commands.cmdInvCtrl
{


    public class createInvCtrlICCCommandValidator : AbstractValidator<createInvCtrlICCCommand>
    {
        private readonly IInvCtrlRepositoryAsync _InvCtrlRepository;

        public createInvCtrlICCCommandValidator(IInvCtrlRepositoryAsync InvCtrlRepository)
        {

            _InvCtrlRepository = InvCtrlRepository;

            RuleForEach(r => r.Items)
                 .SetValidator(model => new createInvCtrlICCItemValidator(_InvCtrlRepository));
        }
    }


    public class createInvCtrlICCItemValidator : AbstractValidator<createInvCtrlICCCommand.InvCtrlItem>
    {
        private readonly IInvCtrlRepositoryAsync _InvCtrlRepository;

        public createInvCtrlICCItemValidator(IInvCtrlRepositoryAsync InvCtrlRepository)
        {
            this._InvCtrlRepository = InvCtrlRepository;

            RuleFor(r => r.WarehouseID)
             .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);
            RuleFor(r => r.ProductID)
             .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);
            RuleFor(r => r.InvCtrlDate)
                  .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);
        }
    }
}