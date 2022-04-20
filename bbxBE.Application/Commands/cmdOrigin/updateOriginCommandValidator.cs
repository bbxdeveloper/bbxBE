using bbxBE.Application.Consts;
using bbxBE.Application.Interfaces.Repositories;
using bxBE.Application.Commands.cmdOrigin;
using FluentValidation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Commands.cmdOrigin
{

    public class UpdateOriginCommandValidator : AbstractValidator<UpdateOriginCommand>
    {
        private readonly IOriginRepositoryAsync _OriginRepository;

        public UpdateOriginCommandValidator(IOriginRepositoryAsync OriginRepository)
        {
            this._OriginRepository = OriginRepository;

            RuleFor(p => p.ID)
                .GreaterThan(0).WithMessage(bbxBEConsts.FV_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED);

            RuleFor(p => p.OriginCode)
                .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED)
                   .Must(
                         (model, Name) =>
                        {
                            return IsUniqueOriginCode(Name, Convert.ToInt64(model.ID));
                        }
                    ).WithMessage(bbxBEConsts.FV_EXISTS)
                .MaximumLength(80).WithMessage(bbxBEConsts.FV_MAXLEN);

            RuleFor(p => p.OriginDescription)
                .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED)
                .MaximumLength(80).WithMessage(bbxBEConsts.FV_MAXLEN);
        }

        private bool IsUniqueOriginCode(string OriginCode, long ID)
        {
            if (OriginCode.Length != 0)
            {
                return _OriginRepository.IsUniqueOriginCode(OriginCode, ID);
            }
            else
            {
                return true;
            }
        }

    }

}
