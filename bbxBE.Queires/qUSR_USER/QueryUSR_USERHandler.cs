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
    public class QueryUSR_USER : QueryParameter, IRequest<PagedResponse<IEnumerable<Entity>>>
    {
        public string Name { get; set; }
        public string LoginName { get; set; }
    }

    public class QueryUSR_USERHandler : IRequestHandler<QueryUSR_USER, PagedResponse<IEnumerable<Entity>>>
    {
        private readonly IUSR_USERRepositoryAsync _userRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public QueryUSR_USERHandler(IUSR_USERRepositoryAsync userRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<PagedResponse<IEnumerable<Entity>>> Handle(QueryUSR_USER request, CancellationToken cancellationToken)
        {
            var validFilter = request;
            var pagination = request;

            //filtered fields security
            if (!string.IsNullOrEmpty(validFilter.Fields))
            {
                //limit to fields in view model
                validFilter.Fields = _modelHelper.ValidateModelFields<GetUSR_USERViewModel, USR_USER>(validFilter.Fields);
            }
            if (string.IsNullOrEmpty(validFilter.Fields))
            {
                //default fields from view model
                validFilter.Fields = _modelHelper.GetQueryableFields<GetUSR_USERViewModel, USR_USER>();
            }

            // query based on filter
            var entitUsers = await _userRepository.QueryPagedUSR_USERReponseAsync(validFilter);
            var data = entitUsers.data.MapItemsFieldsByMapToAnnotation<GetUSR_USERViewModel>();
            RecordsCount recordCount = entitUsers.recordsCount;

            // response wrapper
            return new PagedResponse<IEnumerable<Entity>>(data, validFilter.PageNumber, validFilter.PageSize, recordCount);
        }
    }
}