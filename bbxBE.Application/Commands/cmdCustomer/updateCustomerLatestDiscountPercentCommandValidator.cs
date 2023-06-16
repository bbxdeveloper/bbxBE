using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common.Consts;
using bxBE.Application.Commands.cmdCustomer;
using FluentValidation;

namespace bbxBE.Application.Commands.cmdCustomer
{

    public class updateCustomerLatestDiscountPercentCommandValidator : AbstractValidator<updateCustomerLatestDiscountPercentCommand>
    {
        private readonly ICustomerRepositoryAsync _customerRepository;

        public updateCustomerLatestDiscountPercentCommandValidator(ICustomerRepositoryAsync customerRepository)
        {
            this._customerRepository = customerRepository;

            RuleFor(p => p.ID)
               .GreaterThan(0).WithMessage(bbxBEConsts.ERR_GREATGHERTHANZERO)
               .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED);

            RuleFor(x => x.LatestDiscountPercent)
                    .Must(x => x >= 0 && x <= 100)
                    .WithMessage(bbxBEConsts.ERR_CUSTOMERLATESTDISCOUNTPERCENT);
        }

    }
}
