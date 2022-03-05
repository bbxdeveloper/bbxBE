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

namespace bbxBE.Application.Queries.qOrigin
{
    public class QueryOrigin : QueryParameter, IRequest<PagedResponse<IEnumerable<Entity>>>
    {
        public string SearchString { get; set; }
    }

    public class QueryOriginHandler : IRequestHandler<QueryOrigin, PagedResponse<IEnumerable<Entity>>>
    {
        private readonly IOriginRepositoryAsync _OriginRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public QueryOriginHandler(IOriginRepositoryAsync OriginRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _OriginRepository = OriginRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<PagedResponse<IEnumerable<Entity>>> Handle(QueryOrigin request, CancellationToken cancellationToken)
        {
            var validFilter = request;
            var pagination = request;
            
      

            // query based on filter
            var entities = await _OriginRepository.QueryPagedOriginAsync(validFilter);
            var data = entities.data.MapItemsFieldsByMapToAnnotation<GetOriginViewModel>();
            RecordsCount recordCount = entities.recordsCount;

            // response wrapper
            return new PagedResponse<IEnumerable<Entity>>(data, validFilter.PageNumber, validFilter.PageSize, recordCount);
        }
    }
}