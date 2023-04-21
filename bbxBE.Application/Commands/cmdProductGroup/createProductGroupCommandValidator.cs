using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common.Consts;
using bxBE.Application.Commands.cmdProductGroup;
using FluentValidation;

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
                    .Must(
                            (model, Name) =>
                         {
                             return IsUniqueProductGroupCode(Name);
                         }
                     ).WithMessage(bbxBEConsts.ERR_EXISTS)
                 .MaximumLength(bbxBEConsts.CodeLen).WithMessage(bbxBEConsts.ERR_MAXLEN);

            RuleFor(p => p.ProductGroupDescription)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
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