using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common.Consts;
using FluentValidation;

namespace bbxBE.Application.Commands.cmdOrigin
{

    public class DeleteOriginCommandValidator : AbstractValidator<DeleteOriginCommand>
    {
        private readonly IOriginRepositoryAsync _OriginRepository;

        public DeleteOriginCommandValidator(IOriginRepositoryAsync OriginRepository)
        {
            _OriginRepository = OriginRepository;
            RuleFor(p => p.ID)
                .GreaterThan(0).WithMessage(bbxBEConsts.ERR_GREATGHERTHANZERO)
                .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED);

        }
    }
}
