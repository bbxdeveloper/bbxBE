using bbxBE.Application.Consts;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Enums;
using bxBE.Application.Commands.cmdInvCtrl;
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

namespace bbxBE.Application.Commands.cmdInvCtrl
{

    public class DeleteInvCtrlCommandValidator : AbstractValidator<DeleteInvCtrlCommand>
    {
        private readonly IInvCtrlRepositoryAsync _InvCtrlRepository;

        public DeleteInvCtrlCommandValidator(IInvCtrlRepositoryAsync InvCtrlRepository)
        {
            this._InvCtrlRepository = InvCtrlRepository;

            RuleFor(r => r.ID)
             .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                  .MustAsync(
                        async (model, Name, cancellation) =>
                        {
                            return await CanDeleteAsync(model.ID, cancellation);
                        }
                    ).WithMessage(bbxBEConsts.ERR_InvCtrl_CANTBEDELETED);

        }


         private async Task<bool> CanDeleteAsync( long ID, CancellationToken cancellationToken)
        {
            return await _InvCtrlRepository.CanDeleteAsync(ID);
        }

    }
}