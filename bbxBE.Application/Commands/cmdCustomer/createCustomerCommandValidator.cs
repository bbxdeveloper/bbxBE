using bbxBE.Application.Commands.cmdImport;
using bbxBE.Application.Consts;
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
                .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED)
                .MaximumLength(80).WithMessage(bbxBEConsts.ERR_MAXLEN);

            RuleFor(p => p.TaxpayerNumber)
                      .MaximumLength(13).WithMessage(bbxBEConsts.ERR_MAXLEN)
                      .Must(IsUniqueTaxpayerId).WithMessage(bbxBEConsts.ERR_EXISTS);

            RuleFor(p => p.CustomerBankAccountNumber)
                       .MaximumLength(30).WithMessage(bbxBEConsts.ERR_MAXLEN)
                       .Must(CheckBankAccount).WithMessage(bbxBEConsts.FV_INVALIDFORMAT);

            RuleFor(p => p.Comment)
                 .MaximumLength(2000).WithMessage(bbxBEConsts.ERR_MAXLEN);


            RuleFor(p => p.IsOwnData)
                       .Must(IsUniqueIsOwnData).WithMessage(bbxBEConsts.ERR_CST_OWNEXISTS);

        }

        private bool CheckBankAccount(string p_CustomerBankAccountNumber)
        {
            return _customerRepository.CheckBankAccount(p_CustomerBankAccountNumber);
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



        private bool CheckBankAccountAsync(string p_CustomerBankAccountNumber)
        {
            return  _customerRepository.CheckBankAccount(p_CustomerBankAccountNumber);
        }

    }

}
