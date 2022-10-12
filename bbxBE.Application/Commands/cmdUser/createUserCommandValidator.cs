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
using bbxBE.Common;

namespace bbxBE.Application.Commands.cmdUser
{

    public class createUserCommandValidator : AbstractValidator<createUserCommand>
    {
        private readonly IUserRepositoryAsync _userRepository;

        public createUserCommandValidator(IUserRepositoryAsync userRepository)
        {
            this._userRepository = userRepository;

            RuleFor(p => p.Name)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED)
                .MaximumLength(80).WithMessage(bbxBEConsts.ERR_MAXLEN)
                .MustAsync(IsUniqueNameAsync).WithMessage(bbxBEConsts.ERR_EXISTS);

            RuleFor(p => p.Email)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED)
                .MaximumLength(80).WithMessage(bbxBEConsts.ERR_MAXLEN)
                .MustAsync(Utils.IsValidEmailAsync).WithMessage(bbxBEConsts.ERR_INVALIDEMAIL);

            RuleFor(p => p.LoginName)
                 .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                 .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED)
                 .MaximumLength(80).WithMessage(bbxBEConsts.ERR_MAXLEN);

            RuleFor(p => p.Comment)
                 .MaximumLength(2000).WithMessage(bbxBEConsts.ERR_MAXLEN);
        }

        private async Task<bool> IsUniqueNameAsync(string p_UserName, CancellationToken cancellationToken)
        {
            return await _userRepository.IsUniqueNameAsync(p_UserName);
        }
  
    }

}
