using bbxBE.Common.Consts;
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

namespace bbxBE.Application.Commands.cmdUser
{

    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        private readonly IUserRepositoryAsync _usrRepository;

        public UpdateUserCommandValidator(IUserRepositoryAsync usrRepository)
        {
            this._usrRepository = usrRepository;

            RuleFor(p => p.ID)
                .GreaterThan(0).WithMessage(bbxBEConsts.ERR_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED);


            RuleFor(p => p.Name)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED)
                .MaximumLength(80).WithMessage(bbxBEConsts.ERR_MAXLEN)
                .MustAsync(
                    async (model,Name, cancellation) =>
                    {
                        return await IsUniqueNameAsync(Name, model.ID, cancellation);
                    }
                ).WithMessage(bbxBEConsts.ERR_EXISTS);

            RuleFor(p => p.Email)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED)
                .MaximumLength(80).WithMessage(bbxBEConsts.ERR_MAXLEN)
                .MustAsync(IsValidEmailAsync).WithMessage(bbxBEConsts.ERR_INVALIDEMAIL);

            RuleFor(p => p.LoginName)
                 .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                 .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED)
                 .MaximumLength(80).WithMessage(bbxBEConsts.ERR_MAXLEN);

            RuleFor(p => p.Comment)
                 .MaximumLength(2000).WithMessage(bbxBEConsts.ERR_MAXLEN);
            
        }

       
        private async Task<bool> IsUniqueNameAsync(string p_UserName, long p_ID, CancellationToken cancellationToken)
        {
                return await _usrRepository.IsUniqueNameAsync(p_UserName, p_ID);
        }
        private async Task<bool> IsValidEmailAsync(string p_Email, CancellationToken cancellationToken)
        {

            ParserOptions po = new ParserOptions();
            po.AllowAddressesWithoutDomain = false;
            po.AddressParserComplianceMode = RfcComplianceMode.Strict;

            return await Task.FromResult(MailboxAddress.TryParse(po, p_Email, out _)).ConfigureAwait(false);
        }
    }

}
