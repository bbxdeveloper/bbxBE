using bbxBE.Common.Consts;
using bbxBE.Application.Interfaces.Repositories;
using bxBE.Application.Commands.cmdLocation;
using FluentValidation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Commands.cmdLocation
{

    public class updateStockLocationCommandValidator : AbstractValidator<UpdateLocationCommand>
    {
        private readonly ILocationRepositoryAsync _LocationRepository;

        public updateStockLocationCommandValidator(ILocationRepositoryAsync LocationRepository)
        {
            this._LocationRepository = LocationRepository;

            RuleFor(p => p.ID)
                .GreaterThan(0).WithMessage(bbxBEConsts.ERR_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED);
        }
    }
}
