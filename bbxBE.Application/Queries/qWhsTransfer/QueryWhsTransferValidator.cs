using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Queries.qWhsTransfer;
using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using FluentValidation;
using System;

namespace bbxBE.Application.Commands.cmdInvoice
{

    public class QueryWhsTransferValidator : AbstractValidator<QueryWhsTransfer>
    {
        public QueryWhsTransferValidator(IWhsTransferRepositoryAsync WhsTransferRepositoryAsync)
        {
            RuleFor(r => r.WhsTransferStatus)
            .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
            .Must(CheckWhsTransferStatus).WithMessage(bbxBEConsts.ERR_INVPAYMENTMETHOD);

        }
        private bool CheckWhsTransferStatus(string whsTransferStatus)
        {
            var valid = Enum.TryParse(whsTransferStatus, out enWhsTransferStatus ws);
            return valid;
        }

    }
}