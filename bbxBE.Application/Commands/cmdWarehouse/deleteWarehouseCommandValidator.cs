using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common.Consts;
using FluentValidation;

namespace bbxBE.Application.Commands.cmdWarehouse
{

    public class DeleteWarehouseCommandValidator : AbstractValidator<DeleteWarehouseCommand>
    {
        private readonly IWarehouseRepositoryAsync _WarehouseRepository;

        public DeleteWarehouseCommandValidator(IWarehouseRepositoryAsync WarehouseRepository)
        {
            _WarehouseRepository = WarehouseRepository;
            RuleFor(p => p.ID)
                .GreaterThan(0).WithMessage(bbxBEConsts.ERR_GREATGHERTHANZERO)
                .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED);

        }
    }
}
