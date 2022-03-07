using bbxBE.Application.Consts;
using bbxBE.Application.Interfaces.Repositories;
using bxBE.Application.Commands.cmdProductGroup;
using FluentValidation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Commands.cmdProductGroup
{

    public class UpdateProductGroupCommandValidator : AbstractValidator<UpdateProductGroupCommand>
    {
        private readonly IProductGroupRepositoryAsync _ProductGroupRepository;

        public UpdateProductGroupCommandValidator(IProductGroupRepositoryAsync ProductGroupRepository)
        {
            this._ProductGroupRepository = ProductGroupRepository;


            RuleFor(p => p.ID)
                .GreaterThan(0).WithMessage(bbxBEConsts.FV_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED);

            RuleFor(p => p.ProductGroupCode)
                .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED)
                   .MustAsync(
                        async (model, Name, cancellation) =>
                        {
                            return await IsUniqueProductGroupCodeAsync(Name, Convert.ToInt64(model.ID), cancellation);
                        }
                    ).WithMessage(bbxBEConsts.FV_EXISTS)
                .MaximumLength(80).WithMessage(bbxBEConsts.FV_MAXLEN);

            RuleFor(p => p.ProductGroupDescription)
                .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED)
                .MaximumLength(80).WithMessage(bbxBEConsts.FV_MAXLEN);
        }

        private async Task<bool> IsUniqueProductGroupCodeAsync(string ProductGroupCode, long ID, CancellationToken cancellationToken)
        {
            if (ProductGroupCode.Length != 0)
            {
                return await _ProductGroupRepository.IsUniqueProductGroupCodeAsync(ProductGroupCode, ID);
            }
            else
            {
                return true;
            }
        }

    }

}
