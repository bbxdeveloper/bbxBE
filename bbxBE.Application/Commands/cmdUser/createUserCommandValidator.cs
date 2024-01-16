using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common;
using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using FluentValidation;
using System;
using System.Threading;
using System.Threading.Tasks;

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
                .MaximumLength(80).WithMessage(bbxBEConsts.ERR_MAXLEN)
                .MustAsync(IsUniqueNameAsync).WithMessage(bbxBEConsts.ERR_EXISTS);

            RuleFor(r => r.UserLevel)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                .Must(CheckUserLevel).WithMessage(bbxBEConsts.ERR_INVPAYMENTMETHOD);

            RuleFor(p => p.Email)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                .MaximumLength(80).WithMessage(bbxBEConsts.ERR_MAXLEN)
                .MustAsync(Utils.IsValidEmailAsync).WithMessage(bbxBEConsts.ERR_INVALIDEMAIL);

            RuleFor(p => p.LoginName)
                 .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                 .MaximumLength(80).WithMessage(bbxBEConsts.ERR_MAXLEN);

            RuleFor(p => p.Comment)
                 .MaximumLength(2000).WithMessage(bbxBEConsts.ERR_MAXLEN);

            RuleFor(p => p.Password)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);

        }

        private async Task<bool> IsUniqueNameAsync(string p_UserName, CancellationToken cancellationToken)
        {
            return await _userRepository.IsUniqueNameAsync(p_UserName);
        }

        private bool CheckUserLevel(string userLevel)
        {
            var valid = Enum.TryParse(userLevel, out enUserLevel ul);
            return valid;
        }
    }

}
