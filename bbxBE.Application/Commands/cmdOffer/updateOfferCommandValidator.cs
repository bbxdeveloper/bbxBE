using bbxBE.Application.Consts;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bxBE.Application.Commands.cmdOffer;
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

namespace bbxBE.Application.Commands.cmdOffer
{

    public class updateCommandValidator : AbstractValidator<UpdateOfferCommand>
    {
        private readonly IOfferRepositoryAsync _OfferRepository;

        public updateCommandValidator(IOfferRepositoryAsync OfferRepository)
        {
            this._OfferRepository = OfferRepository;

            RuleFor(r => r.CustomerID)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);

            RuleFor(r => r.OfferNumber)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);

            RuleFor(r => r.OfferIssueDate)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);

            RuleFor(r => r.OfferVaidityDate)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);

            RuleFor(r => new { r.OfferIssueDate, r.OfferVaidityDate}).Must(m => m.OfferIssueDate <= m.OfferVaidityDate)
                .WithMessage(bbxBEConsts.ERR_OFFER_DATE1);

            //Offerline-ekre is validálást!!
            /*
             RuleFor(p => p.UnitOfMeasure)
                  .MustAsync(CheckUnitOfMEasureAsync).WithMessage(bbxBEConsts.FV_INVUNITOFMEASURE);
            */

        }

    }
}