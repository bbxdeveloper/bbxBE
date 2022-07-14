using bbxBE.Application.Consts;
using bbxBE.Application.Interfaces.Repositories;
using bxBE.Application.Commands.cmdEmail;
using FluentValidation;
using System;
using System.Net.Mail;

namespace bbxBE.Application.Commands.cmdOffer
{

    public class sendOfferEmailCommandValidator : AbstractValidator<sendOfferEmailCommand>
    {

        public sendOfferEmailCommandValidator()
        {
            RuleFor(r => r.From.Email)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);
            RuleFor(r => r.To.Email)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);
            RuleFor(r => r.Subject)
                .NotEmpty().WithMessage(bbxBEConsts.ERR_REQUIRED);
            RuleFor(r => r.From.Email)
                .Must(IsValid).WithMessage(bbxBEConsts.EMAIL_FORMAT_ERROR);
            RuleFor(r => r.To.Email)
                .Must(IsValid).WithMessage(bbxBEConsts.EMAIL_FORMAT_ERROR);
        }

        public bool IsValid(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (FormatException)
            {
                return false;
            }
        }

    }
}