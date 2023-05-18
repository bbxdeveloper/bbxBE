﻿using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using bxBE.Application.Commands.cmdWarehouse;
using FluentValidation;
using System;

namespace bbxBE.Application.Commands.cmdInvoice
{

    public class UpdateWhsTransferValidator : AbstractValidator<UpdateWhsTransferCommand>
    {
        private readonly IWhsTransferRepositoryAsync _WhsTransferRepositoryAsync;

        public UpdateWhsTransferValidator(IWhsTransferRepositoryAsync WhsTransferRepositoryAsync)
        {
            this._WhsTransferRepositoryAsync = WhsTransferRepositoryAsync;

            RuleFor(p => p.ID)
                .GreaterThan(0).WithMessage(bbxBEConsts.ERR_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED);

            RuleFor(r => r.FromWarehouseCode)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);
            RuleFor(r => r.ToWarehouseCode)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);
            RuleFor(r => r.TransferDate)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);

            RuleForEach(r => r.WhsTransferLines)
                 .SetValidator(model => new UpdateWhsTransferLinesValidatror());


        }

    }


    public class UpdateWhsTransferLinesValidatror : AbstractValidator<UpdateWhsTransferCommand.WhsTransferLine>
    {
        public UpdateWhsTransferLinesValidatror()
        {
            RuleFor(p => p.UnitOfMeasure)
                 .Must(CheckUnitOfMEasure).WithMessage((model, field) => string.Format(bbxBEConsts.ERR_INVUNITOFMEASURE2, model.WhsTransferLineNumber, model.ProductCode, model.UnitOfMeasure));
        }

        public bool CheckUnitOfMEasure(string unitOfMeasure)
        {
            var valid = Enum.TryParse(unitOfMeasure, out enUnitOfMeasure uom);
            return valid;
        }

    }

}