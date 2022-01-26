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

namespace bbxBE.Application.Features.Positions.Queries.GetPositions
{
    public class GetUSR_USERQuery : QueryParameter, IRequest<PagedResponse<IEnumerable<Entity>>>
    {
        public string USR_LOGIN { get; set; }
    }

    public class GetAllUSR_USERQueryHandler : IRequestHandler<IGetUSR_USERQuery, PagedResponse<IEnumerable<Entity>>>
    {
        private readonly IUSR_USERRepositoryAsync _positionRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public GetAllUSR_USERQueryHandler(IUSR_USERRepositoryAsync positionRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _positionRepository = positionRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<PagedResponse<IEnumerable<Entity>>> Handle(IGetUSR_USERQuery request, CancellationToken cancellationToken)
        {
            var validFilter = request;
            var pagination = request;
            /*
            //filtered fields security
            if (!string.IsNullOrEmpty(validFilter.Fields))
            {
                //limit to fields in view model
                validFilter.Fields = _modelHelper.ValidateModelFields<GetPositionsViewModel>(validFilter.Fields);
            }
            if (string.IsNullOrEmpty(validFilter.Fields))
            {
                //default fields from view model
                validFilter.Fields = _modelHelper.GetModelFields<GetPositionsViewModel>();
            }
            */
            // query based on filter
            var entityPositions = await _positionRepository.GetPagedUSR_USERReponseAsync(validFilter);
            var data = entityPositions.data;
            RecordsCount recordCount = entityPositions.recordsCount;
            // response wrapper
            return new PagedResponse<IEnumerable<Entity>>(data, validFilter.PageNumber, validFilter.PageSize, recordCount);
        }
    }
}