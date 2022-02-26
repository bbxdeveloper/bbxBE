using bbxBE.Application.Consts;
using bbxBE.Application.Interfaces.Repositories;
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

            RuleFor(p => p.ProductCode)
                .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED)
                   .MustAsync(
                        async (model, Name, cancellation) =>
                        {
                            return await IsUniqueProductCodeAsync(Name, Convert.ToInt64(model.ID), cancellation);
                        }
                    ).WithMessage(bbxBEConsts.FV_EXISTS)
                .MaximumLength(80).WithMessage(bbxBEConsts.FV_LEN80);

            RuleFor(p => p.ProductDescription)
                .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED)
                .MaximumLength(80).WithMessage(bbxBEConsts.FV_LEN80);
        }

        private async Task<bool> IsUniqueProductCodeAsync(string ProductCode, long ID, CancellationToken cancellationToken)
        {
            if (ProductCode.Length != 0)
            {
                return await _ProductRepository.IsUniqueProductCodeAsync(ProductCode, ID);
            }
            else
            {
                return true;
            }
        }

    }

}
