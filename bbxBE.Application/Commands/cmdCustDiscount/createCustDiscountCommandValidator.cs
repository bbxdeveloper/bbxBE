using bbxBE.Common.Consts;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bxBE.Application.Commands.cmdCustDiscount;
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

namespace bbxBE.Application.Commands.cmdCustDiscount
{

    public class createCustDiscountCommandValidator : AbstractValidator<CreateCustDiscountCommand>
    {
        public createCustDiscountCommandValidator()
        {
            RuleFor(r => r.CustomerID)
            .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);

            RuleForEach(r => r.Items)
                 .SetValidator(model => new createCustDiscountItemValidator());
        }

        public class createCustDiscountItemValidator : AbstractValidator<CreateCustDiscountCommand.CustDiscountItem>
        {
 
            public createCustDiscountItemValidator()
            {
                RuleFor(r => r.ProductGroupID)
                 .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);
            }
        }

    }
}