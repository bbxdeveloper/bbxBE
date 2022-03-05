using bbxBE.Application.Consts;
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

namespace bbxBE.Application.Commands.cmdOrigin
{

    public class DeleteOriginCommandValidator : AbstractValidator<DeleteOriginCommand>
    {
        private readonly IOriginRepositoryAsync _OriginRepository;

        public DeleteOriginCommandValidator(IOriginRepositoryAsync OriginRepository)
        {
            _OriginRepository = OriginRepository;
            RuleFor(p => p.ID)
                .GreaterThan(0).WithMessage(bbxBEConsts.FV_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED);

        }
    }
}
