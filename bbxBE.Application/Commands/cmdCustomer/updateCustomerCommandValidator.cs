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
                   .Must(
                        (model, Name, cancellation) =>
                        {
                            return IsUniqueTaxpayerId(Name, Convert.ToInt64(model.ID));
                        }
                    ).WithMessage(bbxBEConsts.FV_EXISTS);

            RuleFor(p => p.CustomerBankAccountNumber)
                       .MaximumLength(30).WithMessage(bbxBEConsts.FV_MAXLEN)
                       .Must(CheckBankAccount).WithMessage(bbxBEConsts.FV_INVALIDFORMAT);

            RuleFor(p => p.Comment)
                 .MaximumLength(2000).WithMessage(bbxBEConsts.FV_MAXLEN);

            RuleFor(p => p.IsOwnData)
                     .Must(
                        (model, Name) =>
                        {
                            return IsUniqueIsOwnData(model.IsOwnData, Convert.ToInt64(model.ID));
                        }
                    ).WithMessage(bbxBEConsts.CST_OWNEXISTS);
        }

        private bool IsUniqueTaxpayerId(string TaxpayerNumber, long ID)
        {

            if (TaxpayerNumber == null || string.IsNullOrWhiteSpace(TaxpayerNumber.Replace("-", "")))
                return true;
            var TaxItems = TaxpayerNumber.Split('-');
            if (TaxItems.Length != 0)
            {
                return _customerRepository.IsUniqueTaxpayerId(TaxItems[0], ID);
            }
            else
            {
                return true;
            }
        }

        private bool IsUniqueIsOwnData(bool IsOwnData, long ID)
        {

            if (!IsOwnData)
                return true;


            return _customerRepository.IsUniqueIsOwnData(ID);
        }

        private bool CheckBankAccount(string p_CustomerBankAccountNumber)
        {
            return _customerRepository.CheckBankAccount(p_CustomerBankAccountNumber);
        }

    }

}
