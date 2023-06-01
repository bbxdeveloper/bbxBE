using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using bxBE.Application.Commands.cmdWhsTransfer;
using FluentValidation;
using System;

namespace bbxBE.Application.Commands.cmdWhsTransfer
{

    public class createWhsTransferValidator : AbstractValidator<CreateWhsTransferCommand>
    {
        private readonly IWhsTransferRepositoryAsync _WhsTransferRepositoryAsync;

        public createWhsTransferValidator(IWhsTransferRepositoryAsync WhsTransferRepositoryAsync)
        {
            this._WhsTransferRepositoryAsync = WhsTransferRepositoryAsync;

            RuleFor(r => r.FromWarehouseCode)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);
            RuleFor(r => r.ToWarehouseCode)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);
            RuleFor(r => r.TransferDate)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);


            RuleForEach(r => r.WhsTransferLines)
                 .SetValidator(model => new createWhsTransferLinesValidatror());



        }

    }


    public class createWhsTransferLinesValidatror : AbstractValidator<CreateWhsTransferCommand.WhsTransferLine>
    {
        public createWhsTransferLinesValidatror()
        {
            RuleFor(p => p.Quantity)
            .GreaterThan(0).WithMessage(bbxBEConsts.ERR_GREATGHERTHANZERO);

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