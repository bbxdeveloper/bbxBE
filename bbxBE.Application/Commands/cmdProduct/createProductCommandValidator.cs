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

            RuleFor(p => p.ProductGroupID)
                 .MustAsync(CheckProductGroupIDAsync).WithMessage(bbxBEConsts.FV_INVPRODUCTCROUPID);
 
            RuleFor(p => p.OriginID)
                 .MustAsync(CheckOriginIDAsync).WithMessage(bbxBEConsts.FV_INVORIGINID);


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
        private async Task<bool> CheckProductGroupIDAsync(long ProductGroupID, CancellationToken cancellationToken)
        {
            if (ProductGroupID != 0)
            {
                return await _ProductRepository.CheckProductGroupIDAsync(ProductGroupID);
            }
            return true;
        }
        private async Task<bool> CheckOriginIDAsync(long OriginID, CancellationToken cancellationToken)
        {
            if (OriginID != 0)
            {
                return await _ProductRepository.CheckOriginIDAsync(OriginID);
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