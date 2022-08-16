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

namespace bbxBE.Application.Commands.cmdCounter
{

    public class createCounterCommandValidator : AbstractValidator<CreateCounterCommand>
    {
        private readonly ICounterRepositoryAsync _CounterRepository;

        public createCounterCommandValidator(ICounterRepositoryAsync CounterRepository)
        {
            this._CounterRepository = CounterRepository;
            RuleFor(p => p.CounterCode)
                 .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                 .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED)
                 .MustAsync(
                        async (model, Name, cancellation) =>
                        {
                            return await IsUniqueCounterCodeAsync(Name, cancellation);
                        }
                    ).WithMessage(bbxBEConsts.ERR_EXISTS)
                .MaximumLength(bbxBEConsts.CodeLen).WithMessage(bbxBEConsts.ERR_MAXLEN);

            RuleFor(p => p.CounterDescription)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED)
                .MaximumLength(bbxBEConsts.DescriptionLen).WithMessage(bbxBEConsts.ERR_MAXLEN);

            RuleFor(p => p.Prefix)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED)
                .MaximumLength(bbxBEConsts.CodeLen).WithMessage(bbxBEConsts.ERR_MAXLEN);

            RuleFor(p => p.NumbepartLength)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED)
                .InclusiveBetween(1, 10).WithMessage(bbxBEConsts.ERR_RANGE);
        }
        /*
                    .WithColumn("NumbepartLength").AsInt64().NotNullable()
                    .WithColumn("Suffix").AsString().NotNullable();

        */
        private async Task<bool> IsUniqueCounterCodeAsync(string CounterCode, CancellationToken cancellationToken)
        {
            if (CounterCode.Length != 0)
            {
                return await _CounterRepository.IsUniqueCounterCodeAsync(CounterCode);
            }
            else
            {
                return true;
            }

        }

    }
}