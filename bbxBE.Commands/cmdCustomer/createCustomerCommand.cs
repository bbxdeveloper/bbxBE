﻿using AutoMapper;
using AutoMapper.Configuration.Conventions;
using bbxBE.Application.BLL;
using bbxBE.Application.Consts;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Domain.Entities;
using bbxBE.Infrastructure.Persistence.Contexts;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Commands.cmdCustomer
{
    public class CreateCustomerCommand : IRequest<Response<Customer>>
    {
        public string CustomerName { get; set; }

        public string CustomerBankAccountNumber { get; set; }

        public bool PrivatePerson { get; set; } = false;
        public string TaxpayerId { get; set; }
        public string VatCode { get; set; }
        public string ThirdStateTaxId { get; set; }
        public string CountryCode { get; set; }
        public string CountyCode { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string AdditionalAddressDetail { get; set; }
        public string Comment { get; set; }


    }

    public class CreateUSR_USERCommandHandler : IRequestHandler<CreateCustomerCommand, Response<Customer>>
    {
        private readonly ICustomerRepositoryAsync _customerRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public CreateUSR_USERCommandHandler(ICustomerRepositoryAsync customerRepository, IMapper mapper, IConfiguration configuration)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<Response<Customer>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            var cust = _mapper.Map<Customer>(request);

            cust.CustomerVatStatus = request.PrivatePerson ? CustomerVatStatusType.PRIVATE_PERSON.ToString() :
                                    string.IsNullOrWhiteSpace(request.CountryCode) || request.CountryCode == bbxBEConsts.CNTRY_HU ?
                                            CustomerVatStatusType.DOMESTIC.ToString()
                                            : CustomerVatStatusType.OTHER.ToString();

            cust.CountryCode = string.IsNullOrWhiteSpace(request.CountryCode) ? bbxBEConsts.CNTRY_HU : cust.CountryCode;

            await _customerRepository.AddAsync(cust);
            return new Response<Customer>(cust);
        }


    }
}
