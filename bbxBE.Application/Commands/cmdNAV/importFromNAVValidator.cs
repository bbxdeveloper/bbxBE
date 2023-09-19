using bbxBE.Common.Consts;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.NAV;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Commands.cmdNAV
{

    public class importFromNAVValidator : AbstractValidator<importFromNAVCommand>
    {

        public importFromNAVValidator()
        {
            RuleFor(p => p.InvoiceDirection)
                 .MustAsync(CheckInvoiceDirectionAsync).WithMessage(bbxBEConsts.NAV_INVDIRECTION);

        }
        private async Task<bool> CheckInvoiceDirectionAsync(string InvoiceDirection, CancellationToken cancellationToken)
        {
            InvoiceDirectionType iv;

            bool valid = Enum.TryParse(InvoiceDirection, out iv);
            return valid;
        }

    }
}