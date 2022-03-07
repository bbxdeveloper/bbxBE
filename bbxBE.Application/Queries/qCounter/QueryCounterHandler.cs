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

namespace bbxBE.Application.Queries.qCounter
{
    public class QueryCounter : QueryParameter, IRequest<PagedResponse<IEnumerable<Entity>>>
    {
        public string SearchString { get; set; }
    }

    public class QueryCounterHandler : IRequestHandler<QueryCounter, PagedResponse<IEnumerable<Entity>>>
    {
        private readonly ICounterRepositoryAsync _CounterRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public QueryCounterHandler(ICounterRepositoryAsync CounterRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _CounterRepository = CounterRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<PagedResponse<IEnumerable<Entity>>> Handle(QueryCounter request, CancellationToken cancellationToken)
        {
            var validFilter = request;
            var pagination = request;
            
            /* TODO: törölni
            //filtered fields security
            if (!string.IsNullOrEmpty(validFilter.Fields))
            {
                //limit to fields in view model
                validFilter.Fields = _modelHelper.ValidateModelFields<GetCounterViewModel, Counter>(validFilter.Fields);
            }
  
            if (string.IsNullOrEmpty(validFilter.Fields))
            {
                //default fields from view model
                validFilter.Fields = _modelHelper.GetQueryableFields<GetCounterViewModel, Counter>();
            }
            */


            // query based on filter
            var entities = await _CounterRepository.QueryPagedCounterAsync(validFilter);
            var data = entities.data.MapItemsFieldsByMapToAnnotation<GetCounterViewModel>();
            RecordsCount recordCount = entities.recordsCount;

            // response wrapper
            return new PagedResponse<IEnumerable<Entity>>(data, validFilter.PageNumber, validFilter.PageSize, recordCount);
        }
    }
}