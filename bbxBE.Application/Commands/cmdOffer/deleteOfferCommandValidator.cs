using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common.Consts;
using FluentValidation;

namespace bbxBE.Application.Commands.cmdOffer
{

    public class DeleteOfferCommandValidator : AbstractValidator<DeleteOfferCommand>
    {
        private readonly IOfferRepositoryAsync _OfferRepository;

        public DeleteOfferCommandValidator(IOfferRepositoryAsync OfferRepository)
        {
            _OfferRepository = OfferRepository;
            RuleFor(p => p.ID)
                .GreaterThan(0).WithMessage(bbxBEConsts.ERR_GREATGHERTHANZERO)
                .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED);

        }
    }
}
