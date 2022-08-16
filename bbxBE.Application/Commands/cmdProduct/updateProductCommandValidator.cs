using bbxBE.Common.Consts;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common.Enums;
using bxBE.Application.Commands.cmdProduct;
using FluentValidation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Commands.cmdProduct
{

    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        private readonly IProductRepositoryAsync _ProductRepository;

        public UpdateProductCommandValidator(IProductRepositoryAsync ProductRepository)
        {
            this._ProductRepository = ProductRepository;
            this._ProductRepository = ProductRepository;


            RuleFor(p => p.ID)
                .GreaterThan(0).WithMessage(bbxBEConsts.ERR_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED);

            RuleFor(p => p.ProductCode)
                 .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                 .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED)
                    .Must(
                         (model, ProductCode) =>
                         {
                             return IsUniqueProductCode(ProductCode, model.ID);
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

        private bool IsUniqueProductCode(string ProductCode, long ID)
        {
            return _ProductRepository.IsUniqueProductCode(ProductCode, ID);
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
        private bool CheckUnitOfMEasure(string UnitOfMeasure)
        {
            enUnitOfMeasure uom;

            var valid = Enum.TryParse(UnitOfMeasure, out uom);
            return valid;
        }
        private async Task<bool> CheckVatRateCodeAsync(string VatRateCode, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(VatRateCode))
            {
                return await _ProductRepository.CheckVatRateCodeAsync(VatRateCode);
            }
            return true;
        }
    }

}
