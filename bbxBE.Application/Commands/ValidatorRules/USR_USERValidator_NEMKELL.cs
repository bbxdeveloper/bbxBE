using bbxBE.Application.Consts;
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
                           .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED)
                           .MaximumLength(80).WithMessage(bbxBEConsts.ERR_MAXLEN);
            //                      .MustAsync(IsUniqueNameAsync).WithMessage(bbxBEConsts.FV_EXISTS);
        }

    }

        /*  public static class MyCustomValidators
          {
              public static IRuleBuilderOptions<T, IList<TElement>> ListMustContainFewerThan<T, TElement>(this IRuleBuilder<T, IList<TElement>> ruleBuilder, int num)
              {
                  return ruleBuilder.Must(list => list.Count < num).WithMessage("The list contains too many items");
              }
          }
  */
        /*
             public static IRuleBuilderOptions<USR_USER, IList<XElement>> FieldValidator<T, TElement>(this IRuleBuilder<USR_USER, IList<TElement>> ruleBuilder)
          {

             RuleFor(p => p.Name)
                  .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                  .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED)
                  .MaximumLength(80).WithMessage(bbxBEConsts.FV_LEN80)
                  .MustAsync(IsUniqueNameAsync).WithMessage(bbxBEConsts.FV_EXISTS);

              RuleFor(p => p.Email)
                  .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                  .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED)
                  .MaximumLength(80).WithMessage(bbxBEConsts.FV_LEN80)
                  .MustAsync(IsValidEmailAsync).WithMessage(bbxBEConsts.FV_INVALIDEMAIL);

              RuleFor(p => p.LoginName)
                   .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                   .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED)
                   .MaximumLength(80).WithMessage(bbxBEConsts.FV_LEN80);

              RuleFor(p => p.Comment)
                   .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                   .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED)
                   .MaximumLength(2000).WithMessage(bbxBEConsts.FV_LEN2000);
          }

          private async Task<bool> IsUniqueNameAsync(string p_USR_NAME, CancellationToken cancellationToken)
          {
              return await _usrRepository.IsUniqueNameAsync(p_USR_NAME);
          }
          private async Task<bool> IsValidEmailAsync(string p_USR_EMAIL, CancellationToken cancellationToken)
          {

              ParserOptions po = new ParserOptions();
              po.AllowAddressesWithoutDomain = false;
              po.AddressParserComplianceMode = RfcComplianceMode.Strict;

              return await Task.FromResult(MailboxAddress.TryParse(po, p_USR_EMAIL, out _)).ConfigureAwait(false);
          }
      }
        */
    }
