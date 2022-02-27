﻿using bbxBE.Application.Consts;
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
                    .MustAsync(
                         async (model, Name, cancellation) =>
                         {
                             return await IsUniqueOriginCodeAsync(Name, cancellation);
                         }
                     ).WithMessage(bbxBEConsts.FV_EXISTS)
                 .MaximumLength(80).WithMessage(bbxBEConsts.FV_LEN80);

            RuleFor(p => p.OriginDescription)
                .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED)
                .MaximumLength(80).WithMessage(bbxBEConsts.FV_LEN80);

        }

        private async Task<bool> IsUniqueOriginCodeAsync(string OriginCode, CancellationToken cancellationToken)
        {
            if (OriginCode.Length != 0)
            {
                return await _OriginRepository.IsUniqueOriginCodeAsync(OriginCode);
            }
            else
            {
                return true;
            }

        }

    }
}