using bbxBE.Common.Consts;
using FluentValidation;

namespace bbxBE.Application.Commands.cmdNAV
{

    public class queryTransactionStatusNAVValidator : AbstractValidator<queryTransactionStatusNAVCommand>
    {

        public queryTransactionStatusNAVValidator()
        {
            RuleFor(p => p.TransactionID)
                   .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);

        }

    }
}