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

namespace bbxBE.Application.Queries.qCustDiscount
{
    public class QueryCustDiscount : QueryParameter, IRequest<PagedResponse<IEnumerable<Entity>>>
    {
        public string SearchString { get; set; }
    }

    public class QueryCustDiscountHandler : IRequestHandler<QueryCustDiscount, PagedResponse<IEnumerable<Entity>>>
    {
        private readonly ICustDiscountRepositoryAsync _CustDiscountRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public QueryCustDiscountHandler(ICustDiscountRepositoryAsync CustDiscountRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _CustDiscountRepository = CustDiscountRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<PagedResponse<IEnumerable<Entity>>> Handle(QueryCustDiscount request, CancellationToken cancellationToken)
        {
            var validFilter = request;
            var pagination = request;
            
 
            // query based on filter
            var entities = await _CustDiscountRepository.QueryPagedCustDiscountAsync(validFilter);
            var data = entities.data.MapItemsFieldsByMapToAnnotation<GetCustDiscountViewModel>();
            RecordsCount recordCount = entities.recordsCount;

            // response wrapper
            return new PagedResponse<IEnumerable<Entity>>(data, validFilter.PageNumber, validFilter.PageSize, recordCount);
        }
    }
}