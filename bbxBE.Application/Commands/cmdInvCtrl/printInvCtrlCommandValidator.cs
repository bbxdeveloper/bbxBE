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

namespace bbxBE.Application.Commands.cmdInvCtrl
{

    public class printInvCtrlCommandValidator : AbstractValidator<PrintInvCtrlCommand>
    {
        private readonly IInvCtrlRepositoryAsync _invCtrlRepository;

        public printInvCtrlCommandValidator(IInvCtrlRepositoryAsync invCtrlRepository)
        {
            _invCtrlRepository = invCtrlRepository;
            RuleFor(p => p.InvCtrlPeriodID)
                .GreaterThan(0).WithMessage(bbxBEConsts.ERR_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED);
            RuleFor(p => p.InvPeriodTitle)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);
            RuleFor(p => p.IsInStock)
                .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED);
        }
    }
}
