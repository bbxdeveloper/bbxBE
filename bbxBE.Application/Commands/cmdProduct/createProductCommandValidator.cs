using bbxBE.Application.Consts;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Enums;
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
                 .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                 .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED)
                    .Must(
                          (model, ProductCode) =>
                         {
                             return IsUniqueProductCode(ProductCode);
                         }
                     ).WithMessage(bbxBEConsts.ERR_EXISTS)
                 .MaximumLength(80).WithMessage(bbxBEConsts.ERR_MAXLEN);

            RuleFor(p => p.Description)
                 .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                 .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED)
                 .MaximumLength(80).WithMessage(bbxBEConsts.ERR_MAXLEN);

            RuleFor(p => p.ProductGroupCode)
                 .MustAsync(CheckProductGroupCodeAsync).WithMessage(bbxBEConsts.ERR_INVPRODUCTCROUPCODE);
 
            RuleFor(p => p.OriginCode)
                 .MustAsync(CheckOriginCodeAsync).WithMessage(bbxBEConsts.ERR_INVORIGINCODE);

            RuleFor(p => p.UnitOfMeasure)
                 .Must(CheckUnitOfMEasure).WithMessage(bbxBEConsts.ERR_INVUNITOFMEASURE);

            RuleFor(p => p.VatRateCode)
                 .MustAsync(CheckVatRateCodeAsync).WithMessage(bbxBEConsts.ERR_INVVATRATECODE);

            RuleFor(p => p.VTSZ)
                 .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                 .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED)
                 .MaximumLength(80).WithMessage(bbxBEConsts.ERR_MAXLEN);

        }

        private bool IsUniqueProductCode(string ProductCode)
        {
            return _ProductRepository.IsUniqueProductCode(ProductCode);
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

        private async Task<bool> CheckVatRateCodeAsync(string VatRateCode, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(VatRateCode))
            {
                return await _ProductRepository.CheckVatRateCodeAsync(VatRateCode);
            }
            return true;
        }
        private bool CheckUnitOfMEasure(string UnitOfMeasure)
        {
            var valid = Enum.TryParse(UnitOfMeasure, out enUnitOfMeasure uom);
            return valid;
        }
    }
}