using bbxBE.Application.Consts;
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
                .GreaterThan(0).WithMessage(bbxBEConsts.FV_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED);

            RuleFor(p => p.ProductCode)
                 .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                 .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED)
                    .Must(
                         (model, ProductCode) =>
                         {
                             return IsUniqueProductCode(ProductCode, model.ID);
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
        private async Task<bool> CheckUnitOfMEasureAsync(string UnitOfMeasure, CancellationToken cancellationToken)
        {
            enUnitOfMeasure uom;

            var valid = Enum.TryParse(UnitOfMeasure, out uom);
            return valid;
        }

    }

}
