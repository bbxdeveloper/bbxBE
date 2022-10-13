using bbxBE.Application.Commands.cmdImport;
using bbxBE.Common.Consts;
using bbxBE.Application.Interfaces.Repositories;
using bxBE.Application.Commands.cmdCustomer;
using FluentValidation;
using System;
using System.Threading;
using System.Threading.Tasks;
using bbxBE.Common;

namespace bbxBE.Application.Commands.cmdCustomer
{

    public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
    {
        private readonly ICustomerRepositoryAsync _customerRepository;

        public UpdateCustomerCommandValidator(ICustomerRepositoryAsync customerRepository)
        {
            this._customerRepository = customerRepository;

            RuleFor(p => p.ID)
               .GreaterThan(0).WithMessage(bbxBEConsts.ERR_REQUIRED)
               .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED);

            RuleFor(p => p.CustomerName)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                .MaximumLength(80).WithMessage(bbxBEConsts.ERR_MAXLEN);


            RuleFor(p => p.TaxpayerNumber)
                   .MaximumLength(13).WithMessage(bbxBEConsts.ERR_MAXLEN)
                   .Must(
                        (model, Name) =>
                        {
                            return IsUniqueTaxpayerId(Name, Convert.ToInt64(model.ID));
                        }
                    ).WithMessage(bbxBEConsts.ERR_EXISTS);

            RuleFor(p => p.CustomerBankAccountNumber)
                       .MaximumLength(30).WithMessage(bbxBEConsts.ERR_MAXLEN)
                       .Must(CheckBankAccount).WithMessage(bbxBEConsts.FV_INVALIDFORMAT);

            RuleFor(p => p.Comment)
                 .MaximumLength(2000).WithMessage(bbxBEConsts.ERR_MAXLEN);

            RuleFor(p => p.IsOwnData)
                     .Must(
                        (model, Name) =>
                        {
                            return IsUniqueIsOwnData(model.IsOwnData, Convert.ToInt64(model.ID));
                        }
                    ).WithMessage(bbxBEConsts.ERR_CST_OWNEXISTS);

            RuleFor(p => p.City)
               .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
               .MaximumLength(255).WithMessage(bbxBEConsts.ERR_MAXLEN);

            RuleFor(p => p.AdditionalAddressDetail)
              .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
              .MaximumLength(255).WithMessage(bbxBEConsts.ERR_MAXLEN);

            RuleFor(p => p.Email)
               .MaximumLength(80).WithMessage(bbxBEConsts.ERR_MAXLEN)
               .MustAsync(Utils.IsValidEmailAsync).WithMessage(bbxBEConsts.ERR_INVALIDEMAIL);


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
