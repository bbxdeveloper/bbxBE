using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common.Consts;
using bxBE.Application.Commands.cmdLocation;
using FluentValidation;
using System;
using System.Threading.Tasks;

namespace bbxBE.Application.Commands.cmdLocation
{

    public class UpdateLocationCommandValidator : AbstractValidator<UpdateLocationCommand>
    {
        private readonly ILocationRepositoryAsync _LocationRepository;

        public UpdateLocationCommandValidator(ILocationRepositoryAsync LocationRepository)
        {
            this._LocationRepository = LocationRepository;

            RuleFor(p => p.ID)
                .GreaterThan(0).WithMessage(bbxBEConsts.ERR_GREATGHERTHANZERO)
                .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED);



            RuleFor(p => p.LocationCode)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                   .MustAsync(
                          async (model, LocationCode, cancellation) =>
                          {
                              return await IsUniqueLocationCodeAsync(LocationCode, Convert.ToInt64(model.ID));
                          }
                    ).WithMessage(bbxBEConsts.ERR_EXISTS)
                .MaximumLength(80).WithMessage(bbxBEConsts.ERR_MAXLEN);

            RuleFor(p => p.LocationDescription)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                .MaximumLength(80).WithMessage(bbxBEConsts.ERR_MAXLEN);
        }

        private async Task<bool> IsUniqueLocationCodeAsync(string LocationCode, long ID)
        {
            if (LocationCode.Length != 0)
            {
                return await _LocationRepository.IsUniqueLocationCodeAsync(LocationCode, ID);
            }
            else
            {
                return true;
            }
        }

    }

}
