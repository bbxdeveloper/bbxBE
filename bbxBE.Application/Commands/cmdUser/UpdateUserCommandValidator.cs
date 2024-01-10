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

    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        private readonly IUserRepositoryAsync _usrRepository;

        public UpdateUserCommandValidator(IUserRepositoryAsync usrRepository)
        {
            this._usrRepository = usrRepository;

            RuleFor(p => p.ID)
                .GreaterThan(0).WithMessage(bbxBEConsts.ERR_GREATGHERTHANZERO)
                .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED);


            RuleFor(p => p.Name)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                .MaximumLength(80).WithMessage(bbxBEConsts.ERR_MAXLEN)
                .MustAsync(
                    async (model, Name, cancellation) =>
                    {
                        return await IsUniqueNameAsync(Name, model.ID, cancellation);
                    }
                ).WithMessage(bbxBEConsts.ERR_EXISTS);

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

        }


        private async Task<bool> IsUniqueNameAsync(string p_UserName, long p_ID, CancellationToken cancellationToken)
        {
            return await _usrRepository.IsUniqueNameAsync(p_UserName, p_ID);
        }

        private bool CheckUserLevel(string userLevel)
        {
            var valid = Enum.TryParse(userLevel, out enUserLevel ul);
            return valid;
        }
    }

}
