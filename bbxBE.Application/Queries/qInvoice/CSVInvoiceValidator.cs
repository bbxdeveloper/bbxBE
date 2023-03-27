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

    public class CSVInvoiceValidatorValidator : AbstractValidator<CSVInvoice>
    {

        public CSVInvoiceValidatorValidator()
        {
            RuleFor(p => p.InvoiceType)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                .Must(CheckInvoiceType)
                .WithMessage((model, field) => string.Format(bbxBEConsts.ERR_INVOICETYPE, model.InvoiceType ));

        }

        public bool CheckInvoiceType(string invoiceType)
        {
            if (string.IsNullOrEmpty(invoiceType))
                return false;

            var valid = Enum.TryParse(invoiceType, out enInvoiceType it);
            return valid;
        }
    }

}