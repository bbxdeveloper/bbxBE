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

namespace bbxBE.Application.Commands.cmdCounter
{

    public class DeleteCounterCommandValidator : AbstractValidator<DeleteCounterCommand>
    {
        private readonly ICounterRepositoryAsync _CounterRepository;

        public DeleteCounterCommandValidator(ICounterRepositoryAsync CounterRepository)
        {
            _CounterRepository = CounterRepository;
            RuleFor(p => p.ID)
                .GreaterThan(0).WithMessage(bbxBEConsts.ERR_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED);

        }
    }
}
