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

namespace bbxBE.Application.Queries.qUSR_USER
{
    public class GetUSR_USER:  IRequest<Entity>
    {
        public long ID { get; set; }
        public string Fields { get; set; }
    }

    public class GetUSR_USERHandler : IRequestHandler<GetUSR_USER, Entity>
    {
        private readonly IUSR_USERRepositoryAsync _positionRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public GetUSR_USERHandler(IUSR_USERRepositoryAsync positionRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _positionRepository = positionRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<Entity> Handle(GetUSR_USER request, CancellationToken cancellationToken)
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
            var entityPositions = await _positionRepository.GetUSR_USERReponseAsync(validFilter);


            var data = entityPositions.MapItemFieldsByMapToAnnotation<GetUSR_USERViewModel>();

            // response wrapper
            return data;
        }
    }
}