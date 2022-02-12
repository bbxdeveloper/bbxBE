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
using bbxBE.Queries.ViewModels;
using bbxBE.Domain.Extensions;

namespace bbxBE.Application.Features.Positions.Queries.GetPositions
{
    public class QueryCustomer : QueryParameter, IRequest<PagedResponse<IEnumerable<Entity>>>
    {
        public string CustomerName { get; set; }
    }

    public class QueryCustomerHandler : IRequestHandler<QueryCustomer, PagedResponse<IEnumerable<Entity>>>
    {
        private readonly ICustomerRepositoryAsync _positionRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public QueryCustomerHandler(ICustomerRepositoryAsync positionRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _positionRepository = positionRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<PagedResponse<IEnumerable<Entity>>> Handle(QueryCustomer request, CancellationToken cancellationToken)
        {
            var validFilter = request;
            var pagination = request;
            
            //filtered fields security
            if (!string.IsNullOrEmpty(validFilter.Fields))
            {
                //limit to fields in view model
                validFilter.Fields = _modelHelper.ValidateModelFields<GetCustomerViewModel>(validFilter.Fields);
            }
            if (string.IsNullOrEmpty(validFilter.Fields))
            {
                //default fields from view model
                validFilter.Fields = _modelHelper.GetModelFields<GetCustomerViewModel>();
            }

            // query based on filter
            var entitUsers = await _positionRepository.QueryPagedCustomerReponseAsync(validFilter);
            var data = entitUsers.data.MapItemsFieldsByMapToAnnotation<GetCustomerViewModel>();
            RecordsCount recordCount = entitUsers.recordsCount;

            // response wrapper
            return new PagedResponse<IEnumerable<Entity>>(data, validFilter.PageNumber, validFilter.PageSize, recordCount);
        }
    }
}