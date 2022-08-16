using bbxBE.Common.Consts;
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
                 .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                 .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED)
                    .Must(
                            (model, Name) =>
                         {
                             return  IsUniqueProductGroupCode(Name);
                         }
                     ).WithMessage(bbxBEConsts.ERR_EXISTS)
                 .MaximumLength(bbxBEConsts.CodeLen).WithMessage(bbxBEConsts.ERR_MAXLEN);

            RuleFor(p => p.ProductGroupDescription)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED)
                .MaximumLength(bbxBEConsts.DescriptionLen).WithMessage(bbxBEConsts.ERR_MAXLEN);

        }

        private bool IsUniqueProductGroupCode(string ProductGroupCode)
        {
            if (ProductGroupCode.Length != 0)
            {
                return _ProductGroupRepository.IsUniqueProductGroupCode(ProductGroupCode);
            }
            else
            {
                return true;
            }

        }

    }
}