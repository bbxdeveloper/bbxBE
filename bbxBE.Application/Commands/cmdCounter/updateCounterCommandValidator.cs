using bbxBE.Common.Consts;
using bbxBE.Application.Interfaces.Repositories;
using bxBE.Application.Commands.cmdCounter;
using FluentValidation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Commands.cmdCounter
{

    public class UpdateCounterCommandValidator : AbstractValidator<UpdateCounterCommand>
    {
        private readonly ICounterRepositoryAsync _CounterRepository;

        public UpdateCounterCommandValidator(ICounterRepositoryAsync CounterRepository)
        {
            this._CounterRepository = CounterRepository;


            RuleFor(p => p.ID)
                .GreaterThan(0).WithMessage(bbxBEConsts.ERR_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED);

            RuleFor(p => p.CounterCode)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                   .MustAsync(
                        async (model, Name, cancellation) =>
                        {
                            return await IsUniqueCounterCodeAsync(Name, Convert.ToInt64(model.ID), cancellation);
                        }
                    ).WithMessage(bbxBEConsts.ERR_EXISTS)
                .MaximumLength(bbxBEConsts.CodeLen).WithMessage(bbxBEConsts.ERR_MAXLEN);

            RuleFor(p => p.CounterDescription)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                .MaximumLength(bbxBEConsts.DescriptionLen).WithMessage(bbxBEConsts.ERR_MAXLEN);
        }

        private async Task<bool> IsUniqueCounterCodeAsync(string CounterCode, long ID, CancellationToken cancellationToken)
        {
            if (CounterCode.Length != 0)
            {
                return await _CounterRepository.IsUniqueCounterCodeAsync(CounterCode, ID);
            }
            else
            {
                return true;
            }
        }

    }

}
