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

    public class createInvCtrlCommandValidator : AbstractValidator<CreateInvCtrlCommand>
    {
        private readonly IInvCtrlRepositoryAsync _InvCtrlRepository;

        public createInvCtrlCommandValidator(IInvCtrlRepositoryAsync InvCtrlRepository)
        {
            this._InvCtrlRepository = InvCtrlRepository;

            RuleFor(r => r.WarehouseID)
             .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);
            RuleFor(r => r.ProductID)
             .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);

            RuleFor(r => r.InvCtrlDate)
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
                            return await IsOverLappedPeriodAsync(model.DateFrom, model.DateTo, cancellation);
                        }
                    ).WithMessage(bbxBEConsts.ERR_InvCtrl_DATE2);


            // RuleFor(r => r.UserID)
            // .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);


      
        }


        private async Task<bool> IsOverLappedPeriodAsync(DateTime DateFrom, DateTime DateTo, CancellationToken cancellationToken)
        {
            return await _InvCtrlRepository.IsOverLappedPeriodAsync(DateFrom, DateTo, null);
        }

    }
}