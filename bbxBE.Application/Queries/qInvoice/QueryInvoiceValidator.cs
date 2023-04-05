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
using System;
using bbxBE.Common.Enums;

namespace bbxBE.Application.Queries.qInvoice
{

    public class QueryInvoiceValidatorValidator : AbstractValidator<QueryInvoice>
    {

        public QueryInvoiceValidatorValidator()
        {
            RuleFor(p => p.InvoiceType)
                .Must(CheckInvoiceType)
                .WithMessage((model, field) => string.Format(bbxBEConsts.ERR_INVOICETYPE));

        }

        public bool CheckInvoiceType(string invoiceType)
        {
            var valid = Enum.TryParse(invoiceType, out enInvoiceType it);
            return valid;
        }
    }

}