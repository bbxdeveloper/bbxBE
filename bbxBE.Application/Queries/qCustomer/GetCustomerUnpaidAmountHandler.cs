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
using bbxBE.Application.Commands.cmdImport;

namespace bbxBE.Application.Queries.qCustomer
{
    public class GetCustomerUnpaidAmount:  IRequest<decimal>
    {
        public long ID { get; set; }
  //      public string Fields { get; set; }
    }

    public class GetCustomerUnpaidAmountHandler : IRequestHandler<GetCustomerUnpaidAmount, decimal>
    {
        private readonly ICustomerRepositoryAsync _positionRepository;
        private readonly IInvoiceRepositoryAsync _invoiceRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public GetCustomerUnpaidAmountHandler(
            ICustomerRepositoryAsync positionRepository, 
            IMapper mapper, IModelHelper modelHelper)
        {
            _positionRepository = positionRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }
        itt tartok
        public Task<decimal> Handle(GetCustomerUnpaidAmount request, CancellationToken cancellationToken)
        {
            var entity = _positionRepository.GetCustomer(request.ID);

            // response wrapper
            decimal d = 0;
            return new Task<decimal>(d);
        }

    }
}