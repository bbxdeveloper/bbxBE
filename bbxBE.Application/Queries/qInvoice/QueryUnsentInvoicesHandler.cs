using AutoMapper;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Application.Wrappers;
using bbxBE.Domain.Entities;
using bbxBE.Domain.Extensions;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Queries.qInvoice
{
    public class QueryUnsentInvoices : QueryParameter, IRequest<PagedResponse<IEnumerable<Entity>>>
    {
    }

    public class QueryUnsentInvoicesHandler : IRequestHandler<QueryUnsentInvoices, PagedResponse<IEnumerable<Entity>>>
    {
        private readonly IInvoiceRepositoryAsync _invoiceRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public QueryUnsentInvoicesHandler(IInvoiceRepositoryAsync invoiceRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<PagedResponse<IEnumerable<Entity>>> Handle(QueryUnsentInvoices request, CancellationToken cancellationToken)
        {


            // query based on filter
            var entities = await _invoiceRepository.QueryPagedUnsentInvoiceAsync(request);
            var data = entities.data.MapItemsFieldsByMapToAnnotation<GetInvoiceViewModel>();
            RecordsCount recordCount = entities.recordsCount;

            // response wrapper
            return new PagedResponse<IEnumerable<Entity>>(data, request.PageNumber, request.PageSize, recordCount);
        }
    }
}