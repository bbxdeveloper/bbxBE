using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common.Consts;
using bxBE.Application.Commands.cmdLocation;
using FluentValidation;

namespace bbxBE.Application.Commands.cmdLocation
{

    public class updateStockLocationCommandValidator : AbstractValidator<UpdateLocationCommand>
    {
        private readonly ILocationRepositoryAsync _LocationRepository;

        public updateStockLocationCommandValidator(ILocationRepositoryAsync LocationRepository)
        {
            this._LocationRepository = LocationRepository;

            RuleFor(p => p.ID)
                .GreaterThan(0).WithMessage(bbxBEConsts.ERR_GREATGHERTHANZERO)
                .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED);
        }
    }
}
