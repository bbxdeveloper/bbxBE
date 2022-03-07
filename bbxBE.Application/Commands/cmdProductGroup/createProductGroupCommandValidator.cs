using bbxBE.Application.Consts;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bxBE.Application.Commands.cmdProductGroup;
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

namespace bbxBE.Application.Commands.cmdProductGroup
{

    public class createProductGroupCommandValidator : AbstractValidator<CreateProductGroupCommand>
    {
        private readonly IProductGroupRepositoryAsync _ProductGroupRepository;

        public createProductGroupCommandValidator(IProductGroupRepositoryAsync ProductGroupRepository)
        {
            this._ProductGroupRepository = ProductGroupRepository;
            RuleFor(p => p.ProductGroupCode)
                 .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                 .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED)
                    .MustAsync(
                         async (model, Name, cancellation) =>
                         {
                             return await IsUniqueProductGroupCodeAsync(Name, cancellation);
                         }
                     ).WithMessage(bbxBEConsts.FV_EXISTS)
                 .MaximumLength(80).WithMessage(bbxBEConsts.FV_MAXLEN);

            RuleFor(p => p.ProductGroupDescription)
                .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED)
                .MaximumLength(80).WithMessage(bbxBEConsts.FV_MAXLEN);

        }

        private async Task<bool> IsUniqueProductGroupCodeAsync(string ProductGroupCode, CancellationToken cancellationToken)
        {
            if (ProductGroupCode.Length != 0)
            {
                return await _ProductGroupRepository.IsUniqueProductGroupCodeAsync(ProductGroupCode);
            }
            else
            {
                return true;
            }

        }

    }
}