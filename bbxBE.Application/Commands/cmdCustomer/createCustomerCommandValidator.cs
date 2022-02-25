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
                .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED)
                .MaximumLength(80).WithMessage(bbxBEConsts.FV_LEN80);

            RuleFor(p => p.CustomerBankAccountNumber)
                     .MaximumLength(30).WithMessage(bbxBEConsts.FV_LEN30);

            RuleFor(p => p.TaxpayerNumber)
                       .MaximumLength(13).WithMessage(bbxBEConsts.FV_LEN30)
                       .MustAsync(IsUniqueTaxpayerIdAsync).WithMessage(bbxBEConsts.FV_EXISTS);

            RuleFor(p => p.CustomerBankAccountNumber)
                       .MaximumLength(30).WithMessage(bbxBEConsts.FV_LEN30)
                       .MustAsync(CheckBankAccountAsync).WithMessage(bbxBEConsts.FV_EXISTS);

            RuleFor(p => p.Comment)
                 .MaximumLength(2000).WithMessage(bbxBEConsts.FV_LEN2000);

        }

        private async Task<bool> IsUniqueTaxpayerIdAsync(string TaxpayerNumber, CancellationToken cancellationToken)
        {
            var TaxItems = TaxpayerNumber.Split('-');
            if (TaxItems.Length != 0)
            {
                return await _customerRepository.IsUniqueTaxpayerIdAsync(TaxItems[0]);
            }
            else
            {
                return true;
            }
        }
        private async Task<bool> CheckBankAccountAsync(string p_CustomerBankAccountNumber, CancellationToken cancellationToken)
        {
            return await _customerRepository.CheckBankAccountAsync(p_CustomerBankAccountNumber);
        }

    }

}
