using bbxBE.Application.Consts;
using bbxBE.Application.Interfaces.Repositories;
using bxBE.Application.Commands.cmdCustomer;
using FluentValidation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Commands.cmdCustomer
{

    public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
    {
        private readonly ICustomerRepositoryAsync _customerRepository;

        public UpdateCustomerCommandValidator(ICustomerRepositoryAsync customerRepository)
        {
            this._customerRepository = customerRepository;

            RuleFor(p => p.ID)
               .GreaterThan(0).WithMessage(bbxBEConsts.FV_REQUIRED)
               .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED);

            RuleFor(p => p.CustomerName)
                .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED)
                .MaximumLength(80).WithMessage(bbxBEConsts.FV_MAXLEN);


            RuleFor(p => p.TaxpayerNumber)
                   .MaximumLength(13).WithMessage(bbxBEConsts.FV_MAXLEN)
                   .MustAsync(
                        async (model, Name, cancellation) =>
                        {
                            return await IsUniqueTaxpayerIdAsync(Name, Convert.ToInt64(model.ID), cancellation);
                        }
                    ).WithMessage(bbxBEConsts.FV_EXISTS);

            RuleFor(p => p.CustomerBankAccountNumber)
                       .MaximumLength(30).WithMessage(bbxBEConsts.FV_MAXLEN)
                       .MustAsync(CheckBankAccountAsync).WithMessage(bbxBEConsts.FV_INVALIDFORMAT);

            RuleFor(p => p.Comment)
                 .MaximumLength(2000).WithMessage(bbxBEConsts.FV_MAXLEN);

            RuleFor(p => p.IsOwnData)
                     .MustAsync(
                        async (model, Name, cancellation) =>
                        {
                            return await IsUniqueIsOwnDataAsync(model.IsOwnData, Convert.ToInt64(model.ID), cancellation);
                        }
                    ).WithMessage(bbxBEConsts.CST_OWNEXISTS);
        }

        private async Task<bool> IsUniqueTaxpayerIdAsync(string TaxpayerNumber, long ID, CancellationToken cancellationToken)
        {

            if (TaxpayerNumber == null || string.IsNullOrWhiteSpace(TaxpayerNumber.Replace("-", "")))
                return true;
            var TaxItems = TaxpayerNumber.Split('-');
            if (TaxItems.Length != 0)
            {
                return await _customerRepository.IsUniqueTaxpayerIdAsync(TaxItems[0], ID);
            }
            else
            {
                return true;
            }
        }

        private async Task<bool> IsUniqueIsOwnDataAsync(bool IsOwnData, long ID, CancellationToken cancellationToken)
        {

            if (!IsOwnData)
                return true;


            return await _customerRepository.IsUniqueIsOwnDataAsync(ID);
        }

        private async Task<bool> CheckBankAccountAsync(string p_CustomerBankAccountNumber, CancellationToken cancellationToken)
        {
            return await _customerRepository.CheckBankAccountAsync(p_CustomerBankAccountNumber);
        }

    }

}
