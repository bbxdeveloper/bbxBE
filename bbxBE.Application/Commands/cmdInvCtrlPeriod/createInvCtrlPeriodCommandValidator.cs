using bbxBE.Application.Consts;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Enums;
using bxBE.Application.Commands.cmdInvCtrlPeriod;
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

namespace bbxBE.Application.Commands.cmdInvCtrlPeriod
{

    public class createInvCtrlPeriodCommandValidator : AbstractValidator<CreateInvCtrlPeriodCommand>
    {
        private readonly IInvCtrlPeriodRepositoryAsync _InvCtrlPeriodRepository;

        public createInvCtrlPeriodCommandValidator(IInvCtrlPeriodRepositoryAsync InvCtrlPeriodRepository)
        {
            this._InvCtrlPeriodRepository = InvCtrlPeriodRepository;

            RuleFor(r => r.WarehouseID)
             .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);

            RuleFor(r => r.DateFrom)
                  .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                  .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED)
                  .MustAsync(
                        async (model, Name, cancellation) =>
                        {
                            return await IsOverLappedPeriodAsync(model.DateFrom, model.DateTo, cancellation);
                        }
                    ).WithMessage(bbxBEConsts.ERR_INVCTRLPERIOD_DATE2);

            RuleFor(r => r.DateTo)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);

            // RuleFor(r => r.UserID)
            // .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);


            RuleFor(r => new { r.DateFrom, r.DateTo }).Must(m => m.DateFrom <= m.DateTo)
                .WithMessage(bbxBEConsts.ERR_INVCTRLPERIOD_DATE1);

        }


        private async Task<bool> IsOverLappedPeriodAsync(DateTime DateFrom, DateTime DateTo, CancellationToken cancellationToken)
        {
            return await _InvCtrlPeriodRepository.IsOverLappedPeriodAsync(DateFrom, DateTo);
        }

    }
}