using AutoMapper;
using AutoMapper.Configuration.Conventions;
using bbxBE.Application.BLL;
using bbxBE.Application.Commands.cmdImport;
using bbxBE.Common.Consts;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using bbxBE.Common.ExpiringData;

namespace bxBE.Application.Commands.cmdCustomer
{
    public class UpdateCustomerCommand : IRequest<Response<Customer>>
    {
        public long ID { get; set; }
        [ColumnLabel("Név")]
        [Description("Ügyfélnév/saját név")]
        public string CustomerName { get; set; }

        [ColumnLabel("Bankszámlaszám")]
        [Description("Bankszámlaszám")]
        public string CustomerBankAccountNumber { get; set; }

        [ColumnLabel("Magánszemély?")]
        [Description("Magánszemély?")]
        public bool PrivatePerson { get; set; } = false;

        [ColumnLabel("Adóalany adószám")]
        [Description("Adóalany adószám")]
        public string TaxpayerNumber { get; set; }          //9999999-9-99

        [ColumnLabel("Külföldi adószám")]
        [Description("Külföldi adószám")]
        public string ThirdStateTaxId { get; set; }
        [ColumnLabel("Országkód")]
        [Description("Országkód")]
        public string CountryCode { get; set; }
        [ColumnLabel("Régiókód")]
        [Description("Régiókód")]
        public string Region { get; set; }
        [ColumnLabel("IRSZ")]
        [Description("Irányítószám")]
        public string PostalCode { get; set; }
        [ColumnLabel("Város")]
        [Description("Város")]
        public string City { get; set; }
        [ColumnLabel("Cím")]
        [Description("Cím")]
        public string AdditionalAddressDetail { get; set; }
        [ColumnLabel("Email")]
        [Description("Email")]
        public string Email { get; set; }

        [ColumnLabel("Eladási ártípus")]
        [Description("Eladási ártípus")]
        public string UnitPriceType { get; set; }

        [ColumnLabel("Megjegyzés")]
        [Description("Megjegyzés")]
        public string Comment { get; set; }

        [ColumnLabel("Saját adat?")]
        [Description("Saját adat? (csak egy ilyen rekord lehet)")]
        public bool IsOwnData { get; set; }

    }

    public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Response<Customer>>
    {
        private readonly ICustomerRepositoryAsync _customerRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IExpiringData<ExpiringDataObject> _expiringData;

        public UpdateCustomerCommandHandler(ICustomerRepositoryAsync customerRepository, 
                        IMapper mapper, 
                        IConfiguration configuration,
                        IExpiringData<ExpiringDataObject> expiringData)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _configuration = configuration;
            _expiringData = expiringData;
        }

        public async Task<Response<Customer>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            var cust = _mapper.Map<Customer>(request);
            cust.CustomerBankAccountNumber = cust.CustomerBankAccountNumber?.ToUpper();
            cust.ThirdStateTaxId = cust.ThirdStateTaxId?.ToUpper();

            await _customerRepository.UpdateCustomerAsync(cust, _expiringData);
            return new Response<Customer>(cust);
        }


    }
}
