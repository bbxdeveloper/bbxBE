using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common.Consts;
using FluentValidation;

namespace bbxBE.Application.Commands.cmdLocation
{

    public class DeleteLocationCommandValidator : AbstractValidator<DeleteLocationCommand>
    {
        private readonly ILocationRepositoryAsync _LocationRepository;

        public DeleteLocationCommandValidator(ILocationRepositoryAsync LocationRepository)
        {
            _LocationRepository = LocationRepository;
            RuleFor(p => p.ID)
                .GreaterThan(0).WithMessage(bbxBEConsts.ERR_GREATGHERTHANZERO)
                .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED);

        }
    }
}
