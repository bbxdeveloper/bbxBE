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
using bbxBE.Application.Commands.cmdImport;

namespace bbxBE.Application.Queries.qCustomer
{
    public class QueryCustomer : QueryParameter, IRequest<PagedResponse<IEnumerable<Entity>>>
    {
        public string SearchString { get; set; }

        public bool? IsOwnData { get; set; }
    }

    public class QueryCustomerHandler : IRequestHandler<QueryCustomer, PagedResponse<IEnumerable<Entity>>>
    {
        private readonly ICustomerRepositoryAsync _customerRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public QueryCustomerHandler(ICustomerRepositoryAsync customerRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<PagedResponse<IEnumerable<Entity>>> Handle(QueryCustomer request, CancellationToken cancellationToken)
        {
            var validFilter = request;
            var pagination = request;
            

            // query based on filter
            var entities = await _customerRepository.QueryPagedCustomerAsync(validFilter);
            var data = entities.data.MapItemsFieldsByMapToAnnotation<GetCustomerViewModel>();
            RecordsCount recordCount = entities.recordsCount;

            // response wrapper
            return new PagedResponse<IEnumerable<Entity>>(data, validFilter.PageNumber, validFilter.PageSize, recordCount);
        }
    }
}