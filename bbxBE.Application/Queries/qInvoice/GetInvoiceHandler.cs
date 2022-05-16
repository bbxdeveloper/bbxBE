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

namespace bbxBE.Application.Queries.qInvoice
{
    public class GetInvoice:  IRequest<Entity>
    {
        public long ID { get; set; }

        //Teljes reációs szerkezet kell? I/N
        public bool FullData { get; set; } = true;
        //      public string Fields { get; set; }
    }

    public class GetInvoiceHandler : IRequestHandler<GetInvoice, Entity>
    {
        private readonly IInvoiceRepositoryAsync _invoiceRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public   GetInvoiceHandler(IInvoiceRepositoryAsync invoiceRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<Entity> Handle(GetInvoice request, CancellationToken cancellationToken)
        {
            var validFilter = request;
            var pagination = request;
          

            // query based on filter
            var entityInvoices = await _invoiceRepository.GetInvoiceAsync(validFilter);
            var data = entityInvoices.MapItemFieldsByMapToAnnotation<GetInvoiceViewModel>();

            // response wrapper
            return data;
        }
    }
}