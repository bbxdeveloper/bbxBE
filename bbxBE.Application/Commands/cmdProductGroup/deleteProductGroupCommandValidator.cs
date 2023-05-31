using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common.Consts;
using FluentValidation;

namespace bbxBE.Application.Commands.cmdProductGroup
{

    public class DeleteProductGroupCommandValidator : AbstractValidator<DeleteProductGroupCommand>
    {
        private readonly IProductGroupRepositoryAsync _ProductGroupRepository;

        public DeleteProductGroupCommandValidator(IProductGroupRepositoryAsync ProductGroupRepository)
        {
            _ProductGroupRepository = ProductGroupRepository;
            RuleFor(p => p.ID)
                .GreaterThan(0).WithMessage(bbxBEConsts.ERR_GREATGHERTHANZERO)
                .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED);

        }
    }
}
