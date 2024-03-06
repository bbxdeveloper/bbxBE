using bbxBE.Common.Consts;
using FluentValidation;

namespace bbxBE.Application.Queries.qInvoice
{

    public class QueryXChangeValidator : AbstractValidator<QueryXChange>
    {

        public QueryXChangeValidator()
        {

            RuleFor(f => f.CreateTimeFrom)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                .LessThanOrEqualTo(f => f.CreateTimeTo.Value).WithMessage(bbxBEConsts.ERR_DATEINTERVAL)
                .When(f => f.CreateTimeTo.HasValue);
        }
    }
}