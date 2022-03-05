using bbxBE.Application.Consts;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bxBE.Application.Commands.cmdWarehouse;
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

namespace bbxBE.Application.Commands.cmdWarehouse
{

    public class createWarehouseCommandValidator : AbstractValidator<CreateWarehouseCommand>
    {
        private readonly IWarehouseRepositoryAsync _WarehouseRepository;

        public createWarehouseCommandValidator(IWarehouseRepositoryAsync WarehouseRepository)
        {
            this._WarehouseRepository = WarehouseRepository;
            RuleFor(p => p.WarehouseCode)
                 .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                 .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED)
                    .MustAsync(
                         async (model, Name, cancellation) =>
                         {
                             return await IsUniqueWarehouseCodeAsync(Name, cancellation);
                         }
                     ).WithMessage(bbxBEConsts.FV_EXISTS)
                 .MaximumLength(80).WithMessage(bbxBEConsts.FV_LEN80);

            RuleFor(p => p.WarehouseDescription)
                .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED)
                .MaximumLength(80).WithMessage(bbxBEConsts.FV_LEN80);

        }

        private async Task<bool> IsUniqueWarehouseCodeAsync(string WarehouseCode, CancellationToken cancellationToken)
        {
            if (WarehouseCode.Length != 0)
            {
                return await _WarehouseRepository.IsUniqueWarehouseCodeAsync(WarehouseCode);
            }
            else
            {
                return true;
            }

        }

    }
}