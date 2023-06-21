using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common.Consts;
using bxBE.Application.Commands.cmdInvCtrlPeriod;
using FluentValidation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Commands.cmdInvCtrlPeriod
{

    public class UpdateInvCtrlPeriodCommandValidator : AbstractValidator<UpdateInvCtrlPeriodCommand>
    {
        private readonly IInvCtrlPeriodRepositoryAsync _InvCtrlPeriodRepository;

        public UpdateInvCtrlPeriodCommandValidator(IInvCtrlPeriodRepositoryAsync InvCtrlPeriodRepository)
        {
            this._InvCtrlPeriodRepository = InvCtrlPeriodRepository;

            RuleFor(r => r.ID)
             .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                  .MustAsync(
                        async (model, Name, cancellation) =>
                        {
                            return await CanUpdateAsync(model.ID, cancellation);
                        }
                    ).WithMessage(bbxBEConsts.ERR_INVCTRLPERIOD_CANTBEUPDATED);

            RuleFor(r => r.WarehouseID)
             .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);

            RuleFor(r => r.DateFrom)
                  .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                  .Must(
                         (model, Name) =>
                         {
                             return model.DateFrom <= model.DateTo;
                         }
                    ).WithMessage(bbxBEConsts.ERR_INVCTRLPERIOD_DATE1)
                  .MustAsync(
                        async (model, Name, cancellation) =>
                        {
                            return await IsOverLappedPeriodAsync(model.DateFrom, model.DateTo, model.ID, model.WarehouseID, cancellation);
                        }
                    ).WithMessage(bbxBEConsts.ERR_INVCTRLPERIOD_DATE2);

            RuleFor(r => r.DateTo)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);

            // RuleFor(r => r.UserID)
            // .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);

        }


        private async Task<bool> IsOverLappedPeriodAsync(DateTime DateFrom, DateTime DateTo, long ID, long WarehouseID, CancellationToken cancellationToken)
        {
            return await _InvCtrlPeriodRepository.IsOverLappedPeriodAsync(DateFrom, DateTo, ID, WarehouseID);
        }
        private async Task<bool> CanUpdateAsync(long ID, CancellationToken cancellationToken)
        {
            return await _InvCtrlPeriodRepository.CanUpdateAsync(ID);
        }

    }
}