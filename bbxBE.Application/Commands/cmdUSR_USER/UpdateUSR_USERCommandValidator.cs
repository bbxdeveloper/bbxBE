using bbxBE.Application.Consts;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
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

namespace bbxBE.Application.Commands.cmdUSR_USER
{

    public class UpdateUSR_USERCommandValidator : AbstractValidator<UpdateUSR_USERCommand>
    {
        private readonly IUSR_USERRepositoryAsync _usrRepository;

        public UpdateUSR_USERCommandValidator(IUSR_USERRepositoryAsync usrRepository)
        {
            this._usrRepository = usrRepository;

            RuleFor(p => p.ID)
                .GreaterThan(0).WithMessage(bbxBEConsts.FV_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED);


            RuleFor(p => p.Name)
                .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED)
                .MaximumLength(80).WithMessage(bbxBEConsts.FV_MAXLEN)
                .MustAsync(
                    async (model,Name, cancellation) =>
                    {
                        return await IsUniqueNameAsync(Name, model.ID, cancellation);
                    }
                ).WithMessage(bbxBEConsts.FV_EXISTS);

            RuleFor(p => p.Email)
                .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED)
                .MaximumLength(80).WithMessage(bbxBEConsts.FV_MAXLEN)
                .MustAsync(IsValidEmailAsync).WithMessage(bbxBEConsts.FV_INVALIDEMAIL);

            RuleFor(p => p.LoginName)
                 .NotEmpty().WithMessage(bbxBEConsts.FV_REQUIRED)
                 .NotNull().WithMessage(bbxBEConsts.FV_REQUIRED)
                 .MaximumLength(80).WithMessage(bbxBEConsts.FV_MAXLEN);

            RuleFor(p => p.Comment)
                 .MaximumLength(2000).WithMessage(bbxBEConsts.FV_MAXLEN);
            
        }

       
        private async Task<bool> IsUniqueNameAsync(string p_USR_NAME, long p_ID, CancellationToken cancellationToken)
        {
                return await _usrRepository.IsUniqueNameAsync(p_USR_NAME, p_ID);
        }
        private async Task<bool> IsValidEmailAsync(string p_USR_EMAIL, CancellationToken cancellationToken)
        {

            ParserOptions po = new ParserOptions();
            po.AllowAddressesWithoutDomain = false;
            po.AddressParserComplianceMode = RfcComplianceMode.Strict;

            return await Task.FromResult(MailboxAddress.TryParse(po, p_USR_EMAIL, out _)).ConfigureAwait(false);
        }
    }

}
