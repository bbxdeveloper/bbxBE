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

namespace bbxBE.Application.Queries.qUser
{
    public class GetUser:  IRequest<Entity>
    {
        public long ID { get; set; }
        public string Fields { get; set; }
    }

    public class GetUserHandler : IRequestHandler<GetUser, Entity>
    {
        private readonly IUserRepositoryAsync _positionRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public GetUserHandler(IUserRepositoryAsync positionRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _positionRepository = positionRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<Entity> Handle(GetUser request, CancellationToken cancellationToken)
        {
            var validFilter = request;
            var pagination = request;

                 /* TODO: törölni
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
*/
            // query based on filter
            var entity = await _positionRepository.GetUserAsync(validFilter.ID, validFilter.Fields);


            var data = entity.MapItemFieldsByMapToAnnotation<GetUsersViewModel>();

            // response wrapper
            return data;
        }
    }
}