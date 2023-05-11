using bbxBE.Common.Consts;
using FluentValidation;

namespace bbxBE.Application.Queries.qStock
{

    public class GetProductStockRecordValidator : AbstractValidator<GetProductStocksRecord>
    {

        public GetProductStockRecordValidator()
        {
            RuleFor(p => p.ProductID)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);
        }

    }
}
