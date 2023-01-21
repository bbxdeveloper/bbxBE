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

namespace bbxBE.Application.Queries.qInvCtrl
{

    public class QueryInvCtrlValidator : AbstractValidator<QueryInvCtrl>
    {
   
        public QueryInvCtrlValidator()
        {
            RuleFor(p => p.InvCtrlPeriodID)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);
        }

    }
}