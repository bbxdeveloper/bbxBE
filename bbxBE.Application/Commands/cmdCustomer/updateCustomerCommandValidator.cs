using bbxBE.Application.BLL;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common;
using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using bxBE.Application.Commands.cmdCustomer;
using FluentValidation;
using System;

namespace bbxBE.Application.Commands.cmdCustomer
{

    public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
    {
        private readonly ICustomerRepositoryAsync _customerRepository;

        public UpdateCustomerCommandValidator(ICustomerRepositoryAsync customerRepository)
        {
            this._customerRepository = customerRepository;

            RuleFor(p => p.ID)
               .GreaterThan(0).WithMessage(bbxBEConsts.ERR_GREATGHERTHANZERO)
               .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED);


            RuleFor(p => p.CustomerName)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                .MaximumLength(80).WithMessage(bbxBEConsts.ERR_MAXLEN);

            RuleFor(p => p.CountryCode)
                       .Must(
                         (model, countryCode) =>
                         {
                             return bllCustomer.ValidateCountryCode(countryCode);
                         }
                       ).WithMessage(bbxBEConsts.ERR_CST_WRONGCOUNTRY)
              .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
              .MaximumLength(2).WithMessage(bbxBEConsts.ERR_MAXLEN);

            RuleFor(p => p.TaxpayerNumber)
                        .Must(
                          (model, TaxpayerNumber) =>
                          {
                              if (model.CountryCode != bbxBEConsts.CNTRY_HU && !string.IsNullOrWhiteSpace(TaxpayerNumber.Replace("-", "")))
                                  return false;
                              return true;
                          }
                        ).WithMessage(bbxBEConsts.ERR_CST_TAXNUMBER_INV)
                        .Must(
                          (model, TaxpayerNumber) =>
                          {
                              if (model.CountryCode != bbxBEConsts.CNTRY_HU)
                                  return true;
                              return CheckTaxPayerNumber(TaxpayerNumber);
                          }
                        ).WithMessage(bbxBEConsts.ERR_CST_TAXNUMBER_INV2)
                   .Must(
                        (model, TaxpayerNumber) =>
                        {
                            return IsUniqueTaxpayerId(TaxpayerNumber, Convert.ToInt64(model.ID));
                        }
                    ).WithMessage(bbxBEConsts.ERR_EXISTS);

            RuleFor(p => p.CustomerBankAccountNumber)
                       .MaximumLength(30).WithMessage(bbxBEConsts.ERR_MAXLEN)
                       .Custom((customerBankAccountNumber, context) =>
                       {
                           var res = CheckBankAccount(customerBankAccountNumber);
                           if (res == enValidateBankAccountResult.ERR_FORMAT)
                           {
                               context.AddFailure(bbxBEConsts.ERR_INVALIDFORMAT.Replace(bbxBEConsts.TOKEN_PROPERTYNAME, context.DisplayName));

                           }
                           if (res == enValidateBankAccountResult.ERR_CHECKSUM)
                           {
                               context.AddFailure(bbxBEConsts.ERR_INVALIDCONTENT.Replace(bbxBEConsts.TOKEN_PROPERTYNAME, context.DisplayName));
                           }
                       });

            RuleFor(p => p.UnitPriceType)
                      .Must(
                          (model, unitPriceType) =>
                          {
                              return bllCustomer.ValidateUnitPriceType(unitPriceType);
                          }
                        ).WithMessage(bbxBEConsts.ERR_CST_WRONGUNITPRICETYPE)
               .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
               .MaximumLength(4).WithMessage(bbxBEConsts.ERR_MAXLEN);

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

            RuleFor(p => p.DefPaymentMethod)
                .Must(
                   (model, defPaymentMethod) =>
                   {
                       return bllCustomer.ValidatePaymentMethod(defPaymentMethod);
                   }
                 ).WithMessage(bbxBEConsts.ERR_CST_WRONGDEFPAYMENTTYPE)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);

            RuleFor(x => x.LatestDiscountPercent)
                    .Must(
                       (model, latestDiscountPercent) =>
                       {
                           return latestDiscountPercent == null || (latestDiscountPercent.Value >= 0 && latestDiscountPercent.Value <= 100);
                       }
                     )
                    .WithMessage(bbxBEConsts.ERR_CUSTOMERLATESTDISCOUNTPERCENT);


        }

        private bool CheckTaxPayerNumber(string p_TaxPayerNumber)
        {
            return _customerRepository.CheckTaxPayerNumber(p_TaxPayerNumber);
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
        private enValidateBankAccountResult CheckBankAccount(string p_CustomerBankAccountNumber)
        {
            return _customerRepository.CheckCustomerBankAccount(p_CustomerBankAccountNumber);
        }

    }

}
