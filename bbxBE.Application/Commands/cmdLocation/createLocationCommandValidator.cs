using bbxBE.Common.Consts;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bxBE.Application.Commands.cmdLocation;
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

namespace bbxBE.Application.Commands.cmdLocation
{

    public class createLocationCommandValidator : AbstractValidator<CreateLocationCommand>
    {
        private readonly ILocationRepositoryAsync _LocationRepository;

        public createLocationCommandValidator(ILocationRepositoryAsync LocationRepository)
        {
            this._LocationRepository = LocationRepository;
            RuleFor(p => p.LocationCode)
                 .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                   .MustAsync(
                          async (model, LocationCode, cancellation) =>
                          {
                              return await IsUniqueLocationCodeAsync(LocationCode);
                          }
                    ).WithMessage(bbxBEConsts.ERR_EXISTS)
                 .MaximumLength(80).WithMessage(bbxBEConsts.ERR_MAXLEN);

            RuleFor(p => p.LocationDescription)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED)
                .MaximumLength(80).WithMessage(bbxBEConsts.ERR_MAXLEN);

        }

        private async Task<bool> IsUniqueLocationCodeAsync(string LocationCode)
        {
            if (LocationCode.Length != 0)
            {
                return await _LocationRepository.IsUniqueLocationCodeAsync(LocationCode);
            }
            else
            {
                return true;
            }

        }

    }
}