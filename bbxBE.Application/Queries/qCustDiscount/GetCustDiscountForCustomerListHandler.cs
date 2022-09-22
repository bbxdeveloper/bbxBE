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
    public class GetCustDiscountForCustomerList:  IRequest<IEnumerable<Entity>>
    {
        public long CustomerID { get; set; }
  //      public string Fields { get; set; }
    }

    public class GetCustDiscountForCustomerListHandler : IRequestHandler<GetCustDiscountForCustomerList, IEnumerable<Entity>>
    {
        private readonly ICustDiscountRepositoryAsync _custDiscountRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public GetCustDiscountForCustomerListHandler(ICustDiscountRepositoryAsync custDiscountRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _custDiscountRepository = custDiscountRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<IEnumerable<Entity>> Handle(GetCustDiscountForCustomerList request, CancellationToken cancellationToken)
        {
            var data =  await _custDiscountRepository.GetCustDiscountForCustomerListAsync(request.CustomerID);
            // response wrapper
            return data;
        }

     
    }
}