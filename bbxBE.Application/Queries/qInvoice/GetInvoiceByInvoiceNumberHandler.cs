using AutoMapper;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Common.Attributes;
using bbxBE.Domain.Entities;
using bbxBE.Domain.Extensions;
using MediatR;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Queries.qInvoice
{
    public class GetInvoiceByInvoiceNumber : IRequest<Entity>
    {

        [ColumnLabel("Bizonylatszám")]
        [Description("Bizonylat sorszáma")]
        public string InvoiceNumber { get; set; }
    }

    public class GetInvoiceByInvoiceNumberHandler : IRequestHandler<GetInvoiceByInvoiceNumber, Entity>
    {
        private readonly IInvoiceRepositoryAsync _invoiceRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public GetInvoiceByInvoiceNumberHandler(IInvoiceRepositoryAsync invoiceRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<Entity> Handle(GetInvoiceByInvoiceNumber request, CancellationToken cancellationToken)
        {

            var entity = await _invoiceRepository.GetInvoiceByInvoiceNumberAsync(request.InvoiceNumber, invoiceQueryTypes.full);
            var data = entity.MapItemFieldsByMapToAnnotation<GetInvoiceViewModel>();

            // response wrapper
            return data;
        }
    }
}