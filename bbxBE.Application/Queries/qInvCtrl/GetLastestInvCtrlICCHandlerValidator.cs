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
using bbxBE.Common.Attributes;
using System.ComponentModel;
using System;
using bbxBE.Common.Consts;
using bxBE.Application.Commands.cmdProductGroup;
using FluentValidation;
using bbxBE.Application.Queries.qInvCtrl;

namespace bbxBE.Application.Queries.qStock
{

    public class GetLastestInvCtrlICCHandlerValidator : AbstractValidator<GetLastestInvCtrlICC>
    {
   
        public GetLastestInvCtrlICCHandlerValidator()
        {
            RuleFor(p => p.WarehouseCode)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);
            RuleFor(p => p.ProductID)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);
        }

    }
}