using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common.Consts;
using FluentValidation;

namespace bbxBE.Application.Commands.cmdOffer
{

    public class printOfferCommandValidator : AbstractValidator<PrintOfferCommand>
    {
        private readonly IOfferRepositoryAsync _offerRepository;

        public printOfferCommandValidator(IOfferRepositoryAsync offerRepository)
        {
            _offerRepository = offerRepository;
            RuleFor(p => p.ID)
                .GreaterThan(0).WithMessage(bbxBEConsts.ERR_GREATGHERTHANZERO)
                .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED);

        }
    }
}
