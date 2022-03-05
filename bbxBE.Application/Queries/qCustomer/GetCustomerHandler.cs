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

namespace bbxBE.Application.Queries.qCustomer
{
    public class GetCustomer:  IRequest<Entity>
    {
        public long ID { get; set; }
  //      public string Fields { get; set; }
    }

    public class GetCustomerHandler : IRequestHandler<GetCustomer, Entity>
    {
        private readonly ICustomerRepositoryAsync _positionRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public GetCustomerHandler(ICustomerRepositoryAsync positionRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _positionRepository = positionRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<Entity> Handle(GetCustomer request, CancellationToken cancellationToken)
        {
            var validFilter = request;
            var pagination = request;
          
            /* TODO: törölni

            if (!string.IsNullOrEmpty(validFilter.Fields))
            {
                //limit to fields in view model
                validFilter.Fields = _modelHelper.ValidateModelFields<GetCustomerViewModel, Customer>(validFilter.Fields);
            }

            if (string.IsNullOrEmpty(validFilter.Fields))
            {
                //default fields from view model
                validFilter.Fields = _modelHelper.GetQueryableFields<GetCustomerViewModel, Customer>();
            }

            */

            // query based on filter
            var entityPositions = await _positionRepository.GetCustomerAsync(validFilter);


            var data = entityPositions.MapItemFieldsByMapToAnnotation<GetCustomerViewModel>();

            // response wrapper
            return data;
        }
    }
}