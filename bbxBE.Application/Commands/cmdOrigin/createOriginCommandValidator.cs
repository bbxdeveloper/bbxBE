using bbxBE.Application.Consts;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bxBE.Application.Commands.cmdOrigin;
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

namespace bbxBE.Application.Commands.cmdOrigin
{

    public class createOriginCommandValidator : AbstractValidator<CreateOriginCommand>
    {
        private readonly IOriginRepositoryAsync _OriginRepository;

        public createOriginCommandValidator(IOriginRepositoryAsync OriginRepository)
        {
            this._OriginRepository = OriginRepository;
            RuleFor(p => p.OriginCode)
                 .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                 .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED)
                    .Must(
                         (model, Name) =>
                         {
                             return  IsUniqueOriginCodeAsync(Name);
                         }
                     ).WithMessage(bbxBEConsts.FV_EXISTS)
                 .MaximumLength(80).WithMessage(bbxBEConsts.FV_MAXLEN);

            RuleFor(p => p.OriginDescription)
                .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED)
                .MaximumLength(80).WithMessage(bbxBEConsts.FV_MAXLEN);

        }

        private bool IsUniqueOriginCodeAsync(string OriginCode)
        {
            if (OriginCode.Length != 0)
            {
                return _OriginRepository.IsUniqueOriginCode(OriginCode);
            }
            else
            {
                return true;
            }

        }

    }
}