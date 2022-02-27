using bbxBE.Application.Consts;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bxBE.Application.Commands.cmdProduct;
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

    public class createProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        private readonly IProductRepositoryAsync _ProductRepository;

        public createProductCommandValidator(IProductRepositoryAsync ProductRepository)
        {
            this._ProductRepository = ProductRepository;
            /*
            RuleFor(p => p.ProductCode)
                 .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                 .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED)
                    .MustAsync(
                         async (model, Name, cancellation) =>
                         {
                             return await IsUniqueProductCodeAsync(Name, cancellation);
                         }
                     ).WithMessage(bbxBEConsts.FV_EXISTS)
                 .MaximumLength(80).WithMessage(bbxBEConsts.FV_LEN80);

            RuleFor(p => p.ProductDescription)
                .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED)
                .MaximumLength(80).WithMessage(bbxBEConsts.FV_LEN80);
            */

        }

        private async Task<bool> IsUniqueProductCodeAsync(string ProductCode, CancellationToken cancellationToken)
        {
            if (ProductCode.Length != 0)
            {
                return await _ProductRepository.IsUniqueProductCodeAsync(ProductCode);
            }
            else
            {
                return true;
            }

        }

    }
}