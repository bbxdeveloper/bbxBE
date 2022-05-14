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

    public class createOfferCommandValidator : AbstractValidator<CreateOfferCommand>
    {
        private readonly IOfferRepositoryAsync _OfferRepository;

        public createOfferCommandValidator(IOfferRepositoryAsync OfferRepository)
        {
            this._OfferRepository = OfferRepository;

            RuleFor(r => r.CustomerID)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);

            RuleFor(r => r.OfferIssueDate)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);

            RuleFor(r => r.OfferVaidityDate)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);


            RuleFor(r => new { r.OfferIssueDate, r.OfferVaidityDate}).Must(m => m.OfferIssueDate <= m.OfferVaidityDate)
                .WithMessage(bbxBEConsts.ERR_OFFER_DATE1);

            RuleFor(p => p.OfferCode)
                 .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                 .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED)
                    .Must(
                         (model, Name) =>
                         {
                             return  IsUniqueOfferCodeAsync(Name);
                         }
                     ).WithMessage(bbxBEConsts.FV_EXISTS)
                 .MaximumLength(80).WithMessage(bbxBEConsts.FV_MAXLEN);

            RuleFor(p => p.OfferDescription)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED)
                .MaximumLength(80).WithMessage(bbxBEConsts.FV_MAXLEN);

        }

        private bool IsUniqueOfferCodeAsync(string OfferCode)
        {
            if (OfferCode.Length != 0)
            {
                return _OfferRepository.IsUniqueOfferCode(OfferCode);
            }
            else
            {
                return true;
            }

        }

    }
}