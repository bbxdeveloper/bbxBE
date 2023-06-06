using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common.Consts;
using FluentValidation;

namespace bbxBE.Application.Commands.cmdInvoice
{

    public class PrintInvoiceCommandValidator : AbstractValidator<PrintInvoiceCommand>
    {
        private readonly IInvoiceRepositoryAsync _invoiceRepository;

        public PrintInvoiceCommandValidator(IInvoiceRepositoryAsync invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
            RuleFor(p => p.ID)
                .GreaterThan(0).WithMessage(bbxBEConsts.ERR_GREATGHERTHANZERO)
                .NotNull().WithMessage(bbxBEConsts.ERR_REQUIRED);

        }
    }
}
