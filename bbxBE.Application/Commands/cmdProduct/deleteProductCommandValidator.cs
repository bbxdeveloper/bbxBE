using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common.Consts;
using FluentValidation;

namespace bbxBE.Application.Commands.cmdProduct
{

    public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
    {
        private readonly IProductRepositoryAsync _ProductRepository;

        public DeleteProductCommandValidator(IProductRepositoryAsync ProductRepository)
        {
            _ProductRepository = ProductRepository;
            RuleFor(p => p.ID)
                .GreaterThan(0).WithMessage(bbxBEConsts.ERR_GREATGHERTHANZERO)
                .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED);

        }
    }
}
