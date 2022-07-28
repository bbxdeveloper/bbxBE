using bbxBE.Application.Consts;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Enums;
using bxBE.Application.Commands.cmdInvCtrl;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Commands.cmdInvCtrl
{

    public class UpdateInvCtrlCommandValidator : AbstractValidator<UpdateInvCtrlCommand>
    {
        private readonly IInvCtrlRepositoryAsync _InvCtrlRepository;

        public UpdateInvCtrlCommandValidator(IInvCtrlRepositoryAsync InvCtrlRepository)
        {
            this._InvCtrlRepository = InvCtrlRepository;

            RuleFor(r => r.ID)
             .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                  .MustAsync(
                        async (model, Name, cancellation) =>
                        {
                            return await CanUpdateAsync(model.ID, cancellation);
                        }
                    ).WithMessage(bbxBEConsts.ERR_InvCtrl_CANTBEDELETED);

            RuleFor(r => r.WarehouseID)
             .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);

            RuleFor(r => r.DateFrom)
                  .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                  .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED)
                  .Must(
                         (model, Name) =>
                         {
                             return model.DateFrom <= model.DateTo;
                         }
                    ).WithMessage(bbxBEConsts.ERR_InvCtrl_DATE1)
                  .MustAsync(
                        async (model, Name, cancellation) =>
                        {
                            return await IsOverLappedPeriodAsync(model.DateFrom, model.DateTo, model.ID, cancellation);
                        }
                    ).WithMessage(bbxBEConsts.ERR_InvCtrl_DATE2);

            RuleFor(r => r.DateTo)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);

            // RuleFor(r => r.UserID)
            // .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);
      
        }


        private async Task<bool> IsOverLappedPeriodAsync(DateTime DateFrom, DateTime DateTo, long ID, CancellationToken cancellationToken)
        {
            return await _InvCtrlRepository.IsOverLappedPeriodAsync(DateFrom, DateTo, ID);
        }
        private async Task<bool> CanUpdateAsync( long ID, CancellationToken cancellationToken)
        {
            return await _InvCtrlRepository.CanUpdateAsync(ID);
        }

    }
}