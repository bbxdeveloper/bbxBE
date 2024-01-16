using bbxBE.Common.Consts;
using FluentValidation;

namespace bbxBE.Application.Queries.qInvoice
{

    public class GetPendigDeliveryNotesSummaryValidator : AbstractValidator<GetPendigDeliveryNotesSummary>
    {

        public GetPendigDeliveryNotesSummaryValidator()
        {
            RuleFor(f => f.WarehouseCode).NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);
        }
    }

}