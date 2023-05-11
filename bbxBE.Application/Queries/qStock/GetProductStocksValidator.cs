using bbxBE.Common.Consts;
using FluentValidation;

namespace bbxBE.Application.Queries.qStock
{

    public class GetProductStocksValidator : AbstractValidator<GetProductStocks>
    {

        public GetProductStocksValidator()
        {
            RuleFor(p => p.ProductID)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);
        }

    }
}