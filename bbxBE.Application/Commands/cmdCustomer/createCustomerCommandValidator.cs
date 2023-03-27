using bbxBE.Application.Commands.cmdImport;
using bbxBE.Common.Consts;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bxBE.Application.Commands.cmdCustomer;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using bbxBE.Common;
using bbxBE.Common.Enums;

namespace bbxBE.Application.Commands.cmdCustomer
{

    public class createCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
    {
        private readonly ICustomerRepositoryAsync _customerRepository;

        public createCustomerCommandValidator(ICustomerRepositoryAsync customerRepository)
        {
            this._customerRepository = customerRepository;


            RuleFor(p => p.CustomerName)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                .MaximumLength(80).WithMessage(bbxBEConsts.ERR_MAXLEN);

            RuleFor(p => p.CountryCode)
                       .Must(CheckCountryCode).WithMessage(bbxBEConsts.ERR_CST_WRONGCOUNTRY)
               .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
               .MaximumLength(255).WithMessage(bbxBEConsts.ERR_MAXLEN);

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
                      .Must(IsUniqueTaxpayerId).WithMessage(bbxBEConsts.ERR_EXISTS);

            RuleFor(p => p.CustomerBankAccountNumber)
                        .MaximumLength(30).WithMessage(bbxBEConsts.ERR_MAXLEN)
                        .Custom((customerBankAccountNumber, context) =>
                        {
                            var res = CheckBankAccount(customerBankAccountNumber);
                            if (res == enValidateBankAccountResult.ERR_FORMAT)
                            {
                                context.AddFailure(bbxBEConsts.ERR_INVALIDFORMAT.Replace( bbxBEConsts.TOKEN_PROPERTYNAME, context.PropertyName));

                            }
                            if (res == enValidateBankAccountResult.ERR_CHECKSUM)
                            {
                                context.AddFailure(bbxBEConsts.ERR_INVALIDCONTENT.Replace(bbxBEConsts.TOKEN_PROPERTYNAME, context.PropertyName));

                            }
                        });

            RuleFor(p => p.Comment)
                 .MaximumLength(2000).WithMessage(bbxBEConsts.ERR_MAXLEN);

            RuleFor(p => p.IsOwnData)
                       .Must(IsUniqueIsOwnData).WithMessage(bbxBEConsts.ERR_CST_OWNEXISTS);

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

        private bool CheckCountryCode(string p_countryCode)
        {
            return _customerRepository.CheckCountryCode(p_countryCode);
        }
        private enValidateBankAccountResult CheckBankAccount(string p_CustomerBankAccountNumber)
        {
            return _customerRepository.CheckCustomerBankAccount(p_CustomerBankAccountNumber);
        }
        private bool CheckTaxPayerNumber(string p_TaxPayerNumber)
        {
            return _customerRepository.CheckTaxPayerNumber(p_TaxPayerNumber);
        }

        private bool IsUniqueTaxpayerId(string TaxpayerNumber)
        {

            if (TaxpayerNumber == null || string.IsNullOrWhiteSpace(TaxpayerNumber.Replace("-", "")))
                return true;

            var TaxItems = TaxpayerNumber.Split('-');
            if (TaxItems.Length != 0)
            {
                return _customerRepository.IsUniqueTaxpayerId(TaxItems[0]);
            }
            else
            {
                return true;
            }
        }
        private bool IsUniqueIsOwnData(bool IsOwnData)
        {

            if (!IsOwnData)
                return true;


            return _customerRepository.IsUniqueIsOwnData();
        }
    }
}
