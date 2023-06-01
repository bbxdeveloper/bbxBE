using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common.Consts;
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
                .GreaterThan(0).WithMessage(bbxBEConsts.ERR_GREATGHERTHANZERO)
                .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED);

            RuleFor(p => p.WarehouseCode)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                   .MustAsync(
                        async (model, Name, cancellation) =>
                        {
                            return await IsUniqueWarehouseCodeAsync(Name, Convert.ToInt64(model.ID), cancellation);
                        }
                    ).WithMessage(bbxBEConsts.ERR_EXISTS)
                .MaximumLength(bbxBEConsts.CodeLen).WithMessage(bbxBEConsts.ERR_MAXLEN);

            RuleFor(p => p.WarehouseDescription)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                .MaximumLength(bbxBEConsts.DescriptionLen).WithMessage(bbxBEConsts.ERR_MAXLEN);
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
