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

namespace bbxBE.Application.Queries.qCustDiscount
{
    public class GetCustDiscount:  IRequest<Entity>
    {
        public long ID { get; set; }
  //      public string Fields { get; set; }
    }

    public class GetCustDiscountHandler : IRequestHandler<GetCustDiscount, Entity>
    {
        private readonly ICustDiscountRepositoryAsync _custDiscountRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public GetCustDiscountHandler(ICustDiscountRepositoryAsync custDiscountRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _custDiscountRepository = custDiscountRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<Entity> Handle(GetCustDiscount request, CancellationToken cancellationToken)
        {
            var validFilter = request;
            var pagination = request;
          

            // query based on filter
            var entityPosition = await _custDiscountRepository.GetCustDiscountAsync(validFilter);
            var data = entityPosition.MapItemFieldsByMapToAnnotation<GetCustDiscountViewModel>();

            // response wrapper
            return data;
        }
    }
}