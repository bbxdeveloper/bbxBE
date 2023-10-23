using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
using bbxBE.Domain.Entities;
using MediatR;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace bxBE.Application.Commands.cmdInvoice
{
    public class UpdatePricePreviewCommand : IRequest<Response<Invoice>>
    {

        [Description("Számlasor")]
        public class InvoiceLine
        {

            [ColumnLabel("ID")]
            [Description("Sor ID")]
            public short ID { get; set; }

            [ColumnLabel("Ár")]
            [Description("Ár")]
            public decimal UnitPrice { get; set; }

        }

        [ColumnLabel("ID")]
        [Description("Bizonylat azonosító")]
        public short ID { get; set; }

        [ColumnLabel("Ügyfél ID")]
        [Description("Ügyfél ID")]
        public long CustomerID { get; set; }

        [ColumnLabel("Felhasználó ID")]
        [Description("Felhasználó ID")]
        public long? UserID { get; set; } = 0;

        [ColumnLabel("Számlasorok")]
        [Description("Számlasorok")]
        public List<UpdatePricePreviewCommand.InvoiceLine> InvoiceLines { get; set; } = new List<UpdatePricePreviewCommand.InvoiceLine>();

    }

    public class UpdatePricePreviewCommandHandler : IRequestHandler<UpdatePricePreviewCommand, Response<Invoice>>
    {
        private readonly IInvoiceRepositoryAsync _InvoiceRepository;

        public UpdatePricePreviewCommandHandler(IInvoiceRepositoryAsync InvoiceRepository)
        {
            _InvoiceRepository = InvoiceRepository;
        }

        public async Task<Response<Invoice>> Handle(UpdatePricePreviewCommand request, CancellationToken cancellationToken)
        {

            var inv = await _InvoiceRepository.UpdatePricePreviewAsynch(request, cancellationToken);
            return new Response<Invoice>(inv);
        }


    }
}