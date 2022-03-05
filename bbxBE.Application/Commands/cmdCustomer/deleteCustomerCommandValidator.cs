using bbxBE.Application.Consts;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
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

    public class DeleteCustomerCommandValidator : AbstractValidator<DeleteCustomerCommand>
    {
        private readonly ICustomerRepositoryAsync _customerRepository;

        public DeleteCustomerCommandValidator(ICustomerRepositoryAsync customerRepository)
        {
            _customerRepository = customerRepository;
            RuleFor(p => p.ID)
                .GreaterThan(0).WithMessage(bbxBEConsts.FV_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED);

        }
    }
}
