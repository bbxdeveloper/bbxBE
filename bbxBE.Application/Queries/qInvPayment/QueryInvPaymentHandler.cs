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

namespace bbxBE.Application.Queries.qLocation
{
    public class QueryInvPayment : QueryParameter, IRequest<PagedResponse<IEnumerable<Entity>>>
    {
        public string SearchString { get; set; }
    }

    public class QueryInvPaymentHandler : IRequestHandler<QueryInvPayment, PagedResponse<IEnumerable<Entity>>>
    {
        private readonly IInvPaymentRepositoryAsync _invPaymentRepositoryAsync;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public QueryInvPaymentHandler(IInvPaymentRepositoryAsync invPaymentRepositoryAsync, IMapper mapper, IModelHelper modelHelper)
        {
            _invPaymentRepositoryAsync = invPaymentRepositoryAsync;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<PagedResponse<IEnumerable<Entity>>> Handle(QueryInvPayment request, CancellationToken cancellationToken)
        {
            var validFilter = request;
            var pagination = request;



            // query based on filter
            var entities = await _invPaymentRepositoryAsync.QueryPagedInvPaymentAsync(validFilter, cancellationToken);
            var data = entities.data.MapItemsFieldsByMapToAnnotation<GetInvPaymentViewModel>();
            RecordsCount recordCount = entities.recordsCount;

            // response wrapper
            return new PagedResponse<IEnumerable<Entity>>(data, validFilter.PageNumber, validFilter.PageSize, recordCount);
        }
    }
}