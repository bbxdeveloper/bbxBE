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
using bbxBE.Application.Queries.qCustomer;

namespace bbxBE.Application.Queries.qStock
{

    public class QueryTaxPayerFromNAVValidator : AbstractValidator<QueryTaxPayer>
    {
   
        public QueryTaxPayerFromNAVValidator()
        {
            RuleFor(p => p.Taxnumber)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                .Length(8).WithMessage(bbxBEConsts.ERR_NAV_TAXPAYER);
        }

    }
}