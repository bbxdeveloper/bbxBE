using AutoMapper;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
using bbxBE.Common.ExpiringData;
using bbxBE.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace bxBE.Application.Commands.cmdCustomer
{
    public class updateCustomerLatestDiscountPercentCommand : IRequest<Response<Customer>>
    {
        [ColumnLabel("ID")]
        [Description("ID")]
        public long ID { get; set; }


        [ColumnLabel("Legutoljára megadott kedvezmény %")]
        [Description("Legutoljára megadott bizonylatkedvezmény %")]
        public decimal LatestDiscountPercent { get; set; }

    }

    public class updateCustomerLatestDiscountPercentCommandHandler : IRequestHandler<updateCustomerLatestDiscountPercentCommand, Response<Customer>>
    {
        private readonly ICustomerRepositoryAsync _customerRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IExpiringData<ExpiringDataObject> _expiringData;

        public updateCustomerLatestDiscountPercentCommandHandler(ICustomerRepositoryAsync customerRepository,
                        IMapper mapper,
                        IConfiguration configuration,
                        IExpiringData<ExpiringDataObject> expiringData)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _configuration = configuration;
            _expiringData = expiringData;
        }

        public async Task<Response<Customer>> Handle(updateCustomerLatestDiscountPercentCommand request, CancellationToken cancellationToken)
        {

            var cust = await _customerRepository.UpdateCustomerLatestDiscountPercentAsync(request.ID, request.LatestDiscountPercent);
            return new Response<Customer>(cust);
        }


    }
}
