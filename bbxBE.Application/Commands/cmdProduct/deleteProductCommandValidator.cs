using bbxBE.Common.Consts;
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

namespace bbxBE.Application.Commands.cmdProduct
{

    public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
    {
        private readonly IProductRepositoryAsync _ProductRepository;

        public DeleteProductCommandValidator(IProductRepositoryAsync ProductRepository)
        {
            _ProductRepository = ProductRepository;
            RuleFor(p => p.ID)
                .GreaterThan(0).WithMessage(bbxBEConsts.ERR_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED);

        }
    }
}
