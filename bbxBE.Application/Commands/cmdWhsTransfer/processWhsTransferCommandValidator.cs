using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common.Consts;
using bxBE.Application.Commands.cmdWhsTransfer;
using FluentValidation;

namespace bbxBE.Application.Commands.cmdWhsTransfer
{

    public class ProcessWhsTransferCommandValidator : AbstractValidator<ProcessWhsTransferCommand>
    {
        private readonly IWhsTransferRepositoryAsync _WhsTransferRepository;

        public ProcessWhsTransferCommandValidator(IWhsTransferRepositoryAsync WhsTransferRepository)
        {
            _WhsTransferRepository = WhsTransferRepository;
            RuleFor(p => p.ID)
                .GreaterThan(0).WithMessage(bbxBEConsts.ERR_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED);

        }
    }
}
