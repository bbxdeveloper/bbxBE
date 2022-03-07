using bbxBE.Application.Consts;
using bbxBE.Application.Interfaces.Repositories;
using bxBE.Application.Commands.cmdWarehouse;
using FluentValidation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Commands.cmdWarehouse
{

    public class UpdateWarehouseCommandValidator : AbstractValidator<UpdateWarehouseCommand>
    {
        private readonly IWarehouseRepositoryAsync _WarehouseRepository;

        public UpdateWarehouseCommandValidator(IWarehouseRepositoryAsync WarehouseRepository)
        {
            this._WarehouseRepository = WarehouseRepository;


            RuleFor(p => p.ID)
                .GreaterThan(0).WithMessage(bbxBEConsts.FV_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED);

            RuleFor(p => p.WarehouseCode)
                .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED)
                   .MustAsync(
                        async (model, Name, cancellation) =>
                        {
                            return await IsUniqueWarehouseCodeAsync(Name, Convert.ToInt64(model.ID), cancellation);
                        }
                    ).WithMessage(bbxBEConsts.FV_EXISTS)
                .MaximumLength(80).WithMessage(bbxBEConsts.FV_MAXLEN);

            RuleFor(p => p.WarehouseDescription)
                .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED)
                .MaximumLength(80).WithMessage(bbxBEConsts.FV_MAXLEN);
        }

        private async Task<bool> IsUniqueWarehouseCodeAsync(string WarehouseCode, long ID, CancellationToken cancellationToken)
        {
            if (WarehouseCode.Length != 0)
            {
                return await _WarehouseRepository.IsUniqueWarehouseCodeAsync(WarehouseCode, ID);
            }
            else
            {
                return true;
            }
        }

    }

}
