using bbxBE.Application.Consts;
using bbxBE.Application.Enums;
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

            RuleFor(p => p.ProductCode)
                 .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                 .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED)
                    .MustAsync(
                         async (model, ProductCode, cancellation) =>
                         {
                             return await IsUniqueProductCodeAsync(ProductCode, cancellation);
                         }
                     ).WithMessage(bbxBEConsts.FV_EXISTS)
                 .MaximumLength(80).WithMessage(bbxBEConsts.FV_MAXLEN);

            RuleFor(p => p.Description)
                 .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                 .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED)
                 .MaximumLength(80).WithMessage(bbxBEConsts.FV_MAXLEN);

            RuleFor(p => p.ProductGroupCode)
                 .MustAsync(CheckProductGroupCodeAsync).WithMessage(bbxBEConsts.FV_INVPRODUCTCROUPID);
 
            RuleFor(p => p.OriginCode)
                 .MustAsync(CheckOriginCodeAsync).WithMessage(bbxBEConsts.FV_INVORIGINID);


            RuleFor(p => p.UnitOfMeasure)
                 .MustAsync(CheckUnitOfMEasureAsync).WithMessage(bbxBEConsts.FV_INVUNITOFMEASURE);

            RuleFor(p => p.VTSZ)
                 .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                 .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED)
                 .MaximumLength(80).WithMessage(bbxBEConsts.FV_MAXLEN);

        }

        private async Task<bool> IsUniqueProductCodeAsync(string ProductCode, CancellationToken cancellationToken)
        {
                return await _ProductRepository.IsUniqueProductCodeAsync(ProductCode);
        }
        private async Task<bool> CheckProductGroupCodeAsync(string ProductGroupCode, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(ProductGroupCode))
            {
                return await _ProductRepository.CheckProductGroupCodeAsync(ProductGroupCode);
            }
            return true;
        }
        private async Task<bool> CheckOriginCodeAsync(string OriginCode, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(OriginCode))
            {
                return await _ProductRepository.CheckOriginCodeAsync(OriginCode);
            }
            return true;
        }
        private async Task<bool> CheckUnitOfMEasureAsync(string UnitOfMeasure, CancellationToken cancellationToken)
        {
            enUnitOfMeasure uom;

            var valid = Enum.TryParse(UnitOfMeasure, out uom);
            return valid;
        }
    }
}