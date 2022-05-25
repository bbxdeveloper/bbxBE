﻿using bbxBE.Application.Consts;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Enums;
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



            RuleForEach(r => r.OfferLines)
                .SetValidator(model => new CreateOfferLinesCommandValidatror());
        }

    }
    public class CreateOfferLinesCommandValidatror : AbstractValidator<CreateOfferCommand.OfferLine>
    {
        public CreateOfferLinesCommandValidatror()
        {
            RuleFor(p => p.UnitOfMeasure)
                 .Must(CheckUnitOfMEasure).WithMessage((model, field) => string.Format(bbxBEConsts.ERR_INVUNITOFMEASURE2, model.LineNumber, model.ProductCode, model.UnitOfMeasure));
        }

        public bool CheckUnitOfMEasure(string unitOfMeasure)
        {
            var valid = Enum.TryParse(unitOfMeasure, out enUnitOfMeasure uom);
            return valid;
        }

    }

}