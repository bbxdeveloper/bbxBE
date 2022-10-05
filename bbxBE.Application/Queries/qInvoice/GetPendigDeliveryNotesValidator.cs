using AutoMapper;
using MediatR;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Parameters;
using bbxBE.Application.Wrappers;
using bbxBE.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Domain.Extensions;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Application.Commands.cmdImport;
using bbxBE.Common.Consts;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace bbxBE.Application.Queries.qInvoice
{

    public class GetPendigDeliveryNotesValidator : AbstractValidator<GetPendigDeliveryNotes>
    {

        public GetPendigDeliveryNotesValidator()
        {
            RuleFor(f => f.CustomerID).NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);
            RuleFor(f => f.WarehouseCode).NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);
            RuleFor(f => f.CurrencyCode).NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);
        }
    }

}