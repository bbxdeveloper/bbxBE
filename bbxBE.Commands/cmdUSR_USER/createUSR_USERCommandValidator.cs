using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Infrastructure.Persistence.Contexts;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Commands.cmdUSR_USER
{

    public class createUSR_USERCommandValidator : AbstractValidator<createUSR_USERCommand>
    {
        private readonly IUSR_USERRepositoryAsync _usrRepository;

        public createUSR_USERCommandValidator(IUSR_USERRepositoryAsync usrRepository)
        {
            this._usrRepository = usrRepository;

            RuleFor(p => p.USR_NAME)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull()
                .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.")
                .MustAsync(IsUniqueNameAsync).WithMessage("{PropertyName} already exists.");
            RuleFor(p => p.USR_LOGIN)
              .NotEmpty().WithMessage("{PropertyName} is required.");
      }

        private async Task<bool> IsUniqueNameAsync(string p_USR_NAME, CancellationToken cancellationToken)
        {
            return await _usrRepository.IsUniqueNameAsync(p_USR_NAME);
        }
    }
 
}
