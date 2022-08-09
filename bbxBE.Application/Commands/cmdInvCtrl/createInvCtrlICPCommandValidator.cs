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


    public class createInvCtrlICPCommandValidator : AbstractValidator<createInvCtrlICPCommand>
    {
        private readonly IInvCtrlRepositoryAsync _InvCtrlRepository;

        public createInvCtrlICPCommandValidator(IInvCtrlRepositoryAsync InvCtrlRepository)
        {

            _InvCtrlRepository = InvCtrlRepository;

            RuleForEach(r => r.Items)
                 .SetValidator(model => new createInvCtrlItemValidator(_InvCtrlRepository));
        }
    }


    public class createInvCtrlItemValidator : AbstractValidator<createInvCtrlICPCommand.InvCtrlItem>
    {
        private readonly IInvCtrlRepositoryAsync _InvCtrlRepository;

        public createInvCtrlItemValidator(IInvCtrlRepositoryAsync InvCtrlRepository)
        {
            this._InvCtrlRepository = InvCtrlRepository;

            RuleFor(r => r.WarehouseID)
             .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);
            RuleFor(r => r.ProductID)
             .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);
            RuleFor(r => r.InvCtlPeriodID)
             .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);

            RuleFor(r => r.InvCtrlDate)
                  .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                  .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED)
                  .MustAsync(
                        async (model, Name, cancellation) =>
                        {
                            //egyelőre csak leltáridőszaki leltárt kezelünk
                            return await CheckInvCtrlDateAsync(enInvCtrlType.ICP.ToString(), model.InvCtlPeriodID.Value, model.InvCtrlDate, cancellation);
                        }
                    ).WithMessage(bbxBEConsts.ERR_INVCTRL_DATE);

            // RuleFor(r => r.UserID)
            // .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);
        }


        private async Task<bool> CheckInvCtrlDateAsync(string InvCtrlType,  long InvCtlPeriodID, DateTime InvCtrlDate, CancellationToken cancellationToken)
        {
            if (InvCtrlType == enInvCtrlType.ICP.ToString())    //leltáridőszaki leltár
            {
                return await _InvCtrlRepository.CheckInvCtrlDateAsync(InvCtlPeriodID, InvCtrlDate);
            }
            else
            {
                return true;        //folyamatos leltár
            }
        }

    }
}