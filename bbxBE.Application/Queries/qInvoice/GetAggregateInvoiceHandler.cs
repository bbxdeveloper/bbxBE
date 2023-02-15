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
    public class GetAggregateInvoice :  IRequest<Entity>
    {
        public long ID { get; set; }

     }

    public class GetAggregateInvoiceHandler : IRequestHandler<GetAggregateInvoice, Entity>
    {
        private readonly IInvoiceRepositoryAsync _invoiceRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public GetAggregateInvoiceHandler(IInvoiceRepositoryAsync invoiceRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<Entity> Handle(GetAggregateInvoice request, CancellationToken cancellationToken)
        {

            var entity = await _invoiceRepository.GetAggregateInvoiceAsync(request.ID);
            var data = entity.MapItemFieldsByMapToAnnotation<GetAggregateInvoiceViewModel>();

            // response wrapper
            return data;
        }
    }
}