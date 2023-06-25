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

namespace bbxBE.Application.Queries.qProduct
{
    public class QueryProduct : QueryParameter, IRequest<PagedResponse<IEnumerable<Entity>>>
    {
        public string SearchString { get; set; }
        public bool? FilterByCode { get; set; }
        public bool? FilterByName { get; set; }

        public List<long> IDList { get; set; }

    }

    public class QueryProductHandler : IRequestHandler<QueryProduct, PagedResponse<IEnumerable<Entity>>>
    {
        private readonly IProductRepositoryAsync _ProductRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public QueryProductHandler(IProductRepositoryAsync ProductRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _ProductRepository = ProductRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<PagedResponse<IEnumerable<Entity>>> Handle(QueryProduct request, CancellationToken cancellationToken)
        {

            // query based on filter
            var entities = await _ProductRepository.QueryPagedProductAsync(request);

            var data = entities.data.MapItemsFieldsByMapToAnnotation<GetProductViewModel>();
            RecordsCount recordCount = entities.recordsCount;

            // response wrapper
            return new PagedResponse<IEnumerable<Entity>>(data, request.PageNumber, request.PageSize, recordCount);
        }
    }
}