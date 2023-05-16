using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common.Consts;
using FluentValidation;

namespace bbxBE.Application.Commands.cmdWhsTransfer
{

    public class DeleteWhsTransferCommandValidator : AbstractValidator<DeleteWhsTransferCommand>
    {
        private readonly IWhsTransferRepositoryAsync _WhsTransferRepository;

        public DeleteWhsTransferCommandValidator(IWhsTransferRepositoryAsync WhsTransferRepository)
        {
            _WhsTransferRepository = WhsTransferRepository;
            RuleFor(p => p.ID)
                .GreaterThan(0).WithMessage(bbxBEConsts.ERR_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED);

        }
    }
}
