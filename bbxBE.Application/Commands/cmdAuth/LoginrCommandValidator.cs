using bbxBE.Common.Consts;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bxBE.Application.Commands.cmdCounter;
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
using bxBE.Application.Commands.cmdAuth;

namespace bbxBE.Application.Commands.cmdAuth
{

    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED);
            RuleFor(p => p.Password)
                 .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                 .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED);
        }
    }
}