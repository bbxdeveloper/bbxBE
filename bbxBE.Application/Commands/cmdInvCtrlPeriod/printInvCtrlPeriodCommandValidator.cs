using bbxBE.Application.Commands.cmdImport;
using bbxBE.Common.Consts;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
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

    public class printInvCtrlPeriodCommandValidator : AbstractValidator<PrintInvCtrlPeriodCommand>
    {
        private readonly IInvCtrlPeriodRepositoryAsync _invCtrlPeriodRepository;

        public printInvCtrlPeriodCommandValidator(IInvCtrlPeriodRepositoryAsync invCtrlPeriodRepository)
        {
            _invCtrlPeriodRepository = invCtrlPeriodRepository;
            RuleFor(p => p.InvCtrlPeriodID)
                .GreaterThan(0).WithMessage(bbxBEConsts.ERR_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED);
            RuleFor(p => p.InvPeriodTitle)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);
        }
    }
}
