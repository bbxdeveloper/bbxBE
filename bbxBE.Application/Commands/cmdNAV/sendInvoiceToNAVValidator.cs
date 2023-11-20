using bbxBE.Common.Consts;
using FluentValidation;

namespace bbxBE.Application.Commands.cmdNAV
{

    public class sendInvoiceToNAVValidator : AbstractValidator<sendInvoiceToNAVCommand>
    {

        public sendInvoiceToNAVValidator()
        {
            RuleFor(p => p.InvoiceNumber)
                   .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);

        }

    }
}