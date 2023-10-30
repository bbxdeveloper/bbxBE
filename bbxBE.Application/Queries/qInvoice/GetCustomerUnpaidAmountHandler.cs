using AutoMapper;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Queries.qCustomer
{
    public class GetCustomerUnpaidAmount : IRequest<decimal>
    {
        public long CustomerID { get; set; }
        //      public string Fields { get; set; }
    }

    public class GetCustomerUnpaidAmountHandler : IRequestHandler<GetCustomerUnpaidAmount, decimal>
    {
        private readonly IInvoiceRepositoryAsync _invoiceRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public GetCustomerUnpaidAmountHandler(
            IInvoiceRepositoryAsync invoiceRepository,
        IMapper mapper, IModelHelper modelHelper)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }
        public async Task<decimal> Handle(GetCustomerUnpaidAmount request, CancellationToken cancellationToken)
        {
            var d = await _invoiceRepository.GetUnPaidAmountAsyn(request.CustomerID);
            return d;
        }
    }
}