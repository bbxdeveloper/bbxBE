using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common.Consts;
using FluentValidation;

namespace bbxBE.Application.Commands.cmdCounter
{

    public class DeleteCounterCommandValidator : AbstractValidator<DeleteCounterCommand>
    {
        private readonly ICounterRepositoryAsync _CounterRepository;

        public DeleteCounterCommandValidator(ICounterRepositoryAsync CounterRepository)
        {
            _CounterRepository = CounterRepository;
            RuleFor(p => p.ID)
                .GreaterThan(0).WithMessage(bbxBEConsts.ERR_GREATGHERTHANZERO)
                .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED);

        }
    }
}
