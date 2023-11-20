using bbxBE.Common.Consts;
using FluentValidation;

namespace bbxBE.Application.Commands.cmdNAV
{

    public class callManageInvoiceNAVValidator : AbstractValidator<callManageInvoiceNAVCommand>
    {

        public callManageInvoiceNAVValidator()
        {
            RuleFor(p => p.InvoiceNumber)
                   .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);

        }

    }
}