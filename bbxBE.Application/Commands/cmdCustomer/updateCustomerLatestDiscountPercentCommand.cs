using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
using bbxBE.Domain.Entities;
using MediatR;
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

        public updateCustomerLatestDiscountPercentCommandHandler(ICustomerRepositoryAsync customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<Response<Customer>> Handle(updateCustomerLatestDiscountPercentCommand request, CancellationToken cancellationToken)
        {

            var cust = await _customerRepository.UpdateCustomerLatestDiscountPercentAsync(request.ID, request.LatestDiscountPercent);
            return new Response<Customer>(cust);
        }


    }
}
