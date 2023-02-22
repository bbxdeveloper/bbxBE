using AutoMapper;
using MediatR;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Parameters;
using bbxBE.Application.Wrappers;
using bbxBE.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Domain.Extensions;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Common.Attributes;
using System.ComponentModel;

namespace bbxBE.Application.Queries.qInvoice
{
    public class GetAggregateInvoiceDeliveryNote :  IRequest<List<Entity>>
    {
        public long InvoiceID { get; set; }
        public long DeliveryNoteInvoiceID { get; set; }

    }

    public class GetAggregateInvoiceDeliveryNoteHandler : IRequestHandler<GetAggregateInvoiceDeliveryNote, List<Entity>>
    {
        private readonly IInvoiceLineRepositoryAsync _invoiceLineRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public GetAggregateInvoiceDeliveryNoteHandler(IInvoiceLineRepositoryAsync invoiceLineRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _invoiceLineRepository = invoiceLineRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<List<Entity>> Handle(GetAggregateInvoiceDeliveryNote request, CancellationToken cancellationToken)
        {
            var result = await _invoiceLineRepository.GetInvoiceLinesByRelDeliveryNoteIDAsync(request.InvoiceID, request.DeliveryNoteInvoiceID);
            return result;
        }
    }
}