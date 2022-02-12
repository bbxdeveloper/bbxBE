using bbxBE.Application.Consts;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Commands.cmdCustomer;
using bbxBE.Infrastructure.Persistence.Contexts;
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

namespace bbxBE.Commands.cmdCustomer
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

            RuleFor(p => p.CustomerBankAccountNumber)
                       .MaximumLength(30).WithMessage(bbxBEConsts.FV_LEN30);

            RuleFor(p => p.TaxpayerId)
                 .Length(1,8).When(p => !string.IsNullOrEmpty(p.TaxpayerId)).WithMessage(bbxBEConsts.FV_LEN8)
                .MustAsync(IsUniqueTaxpayerIdAsync).WithMessage(bbxBEConsts.FV_EXISTS);
/*
            RuleFor(p => p.TaxpayerId)
                .Empty()
                .Length(1).WithMessage(bbxBEConsts.FV_LEN1);

            RuleFor(p => p.TaxpayerId)
                .Empty()
                .Length(2).WithMessage(bbxBEConsts.FV_LEN2);
*/
            RuleFor(p => p.Comment)
                 .MaximumLength(2000).WithMessage(bbxBEConsts.FV_LEN2000);
        }

        private async Task<bool> IsUniqueTaxpayerIdAsync(string p_TaxpayerId, CancellationToken cancellationToken)
        {
            return await _customerRepository.IsUniqueTaxpayerIdAsync(p_TaxpayerId);
        }
       
    }

}
