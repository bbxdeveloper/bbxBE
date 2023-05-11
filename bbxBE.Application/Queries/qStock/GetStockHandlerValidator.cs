using bbxBE.Common.Consts;
using FluentValidation;

namespace bbxBE.Application.Queries.qStock
{

    public class GetStockValidator : AbstractValidator<GetStock>
    {

        public GetStockValidator()
        {
            RuleFor(p => p.ID)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);
        }

    }
}