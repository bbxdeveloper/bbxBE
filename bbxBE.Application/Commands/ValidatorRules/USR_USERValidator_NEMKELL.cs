using bbxBE.Common.Consts;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Domain.Entities;
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
using System.Xml.Linq;

namespace bbxBE.Application.Commands.ValidatorRules
{

    public static class Extensions
    {

        public static IRuleBuilderOptions<string, string> NameValidator<T>(this IRuleBuilder<string, string> ruleBuilder)
        {

           return ruleBuilder.NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                           .MaximumLength(80).WithMessage(bbxBEConsts.ERR_MAXLEN);
            //                      .MustAsync(IsUniqueNameAsync).WithMessage(bbxBEConsts.FV_EXISTS);
        }

    }
    }
