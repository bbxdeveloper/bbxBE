using AutoMapper;
using AutoMapper.Configuration.Conventions;
using bbxBE.Application.BLL;
using bbxBE.Application.Consts;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bxBE.Application.Commands.cmdInvoice
{
    public class CreateInvoiceCommand : IRequest<Response<Invoice>>
    {
        public string InvoiceCode { get; set; }
        public string InvoiceDescription { get; set; }
        public string WarehouseCode { get; set; }
        public string Prefix { get; set; }
        public long CurrentNumber { get; set; }
        public int NumbepartLength { get; set; }
        public string Suffix { get; set; }

    }

    public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, Response<Invoice>>
    {
        private readonly IInvoiceRepositoryAsync _InvoiceRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public CreateInvoiceCommandHandler(IInvoiceRepositoryAsync InvoiceRepository, IMapper mapper, IConfiguration configuration)
        {
            _InvoiceRepository = InvoiceRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<Response<Invoice>> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
        {
            var cnt = _mapper.Map<Invoice>(request);

            cnt = await _InvoiceRepository.AddInvoiceAsync(cnt, request.WarehouseCode);
            return new Response<Invoice>(cnt);
        }


    }
}
