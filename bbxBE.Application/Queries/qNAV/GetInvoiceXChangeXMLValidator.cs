using bbxBE.Common.Consts;
using FluentValidation;

namespace bbxBE.Application.Queries.qInvoice
{

    public class GetInvoiceXChangeXMLValidator : AbstractValidator<GetInvoiceXChangeXML>
    {
        public GetInvoiceXChangeXMLValidator()
        {
            RuleFor(p => p.ID)
                .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED);
        }
    }

}