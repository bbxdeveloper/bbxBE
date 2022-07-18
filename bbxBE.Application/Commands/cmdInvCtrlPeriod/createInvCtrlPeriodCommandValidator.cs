using bbxBE.Application.Consts;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Enums;
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

            RuleFor(r => r.CustomerID)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);

            RuleFor(r => r.InvCtrlPeriodIssueDate)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);

            RuleFor(r => r.InvCtrlPeriodVaidityDate)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);


            RuleFor(r => new { r.InvCtrlPeriodIssueDate, r.InvCtrlPeriodVaidityDate}).Must(m => m.InvCtrlPeriodIssueDate <= m.InvCtrlPeriodVaidityDate)
                .WithMessage(bbxBEConsts.ERR_InvCtrlPeriod_DATE1);



            RuleForEach(r => r.InvCtrlPeriodLines)
                .SetValidator(model => new CreateInvCtrlPeriodLinesCommandValidatror());
        }

    }
    public class CreateInvCtrlPeriodLinesCommandValidatror : AbstractValidator<CreateInvCtrlPeriodCommand.InvCtrlPeriodLine>
    {
        public CreateInvCtrlPeriodLinesCommandValidatror()
        {
            RuleFor(p => p.UnitOfMeasure)
                 .Must(CheckUnitOfMEasure).WithMessage((model, field) => string.Format(bbxBEConsts.ERR_INVUNITOFMEASURE2, model.LineNumber, model.ProductCode, model.UnitOfMeasure));
            RuleFor(p => p.Discount)
               .InclusiveBetween(0, 100)
               .WithMessage((model, field) => string.Format(bbxBEConsts.ERR_DETAIL_PREF, model.LineNumber, model.ProductCode) + bbxBEConsts.ERR_DISCOUNT);
        }

        public bool CheckUnitOfMEasure(string unitOfMeasure)
        {
            var valid = Enum.TryParse(unitOfMeasure, out enUnitOfMeasure uom);
            return valid;
        }

    }

}