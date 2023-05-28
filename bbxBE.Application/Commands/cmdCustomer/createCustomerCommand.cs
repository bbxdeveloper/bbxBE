using AutoMapper;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
using bbxBE.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace bxBE.Application.Commands.cmdCustomer
{
    public class CreateCustomerCommand : IRequest<Response<Customer>>
    {
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

        [ColumnLabel("Fizetési határidő")]
        [Description("Fizetési határidő (napban)")]
        public short PaymentDays { get; set; }

        [ColumnLabel("Figyelmeztetés limit")]
        [Description("Figyelmeztetés limit")]
        public decimal? WarningLimit { get; set; }

        [ColumnLabel("Maximális limit")]
        [Description("Maximális limit")]
        public decimal? MaxLimit { get; set; }

        [ColumnLabel("Alap.fiz.mód")]
        [Description("Alapértelmezett fizetési mód")]
        public string DefPaymentMethod { get; set; }


        [ColumnLabel("Megjegyzés")]
        [Description("Megjegyzés")]
        public string Comment { get; set; }

        [ColumnLabel("Saját adat?")]
        [Description("Saját adat? (csak egy ilyen rekord lehet)")]
        public bool IsOwnData { get; set; }

    }

    public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Response<Customer>>
    {
        private readonly ICustomerRepositoryAsync _customerRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public CreateCustomerCommandHandler(ICustomerRepositoryAsync customerRepository, IMapper mapper, IConfiguration configuration)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<Response<Customer>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            var cust = _mapper.Map<Customer>(request);
            cust.CustomerBankAccountNumber = cust.CustomerBankAccountNumber?.ToUpper();
            cust.ThirdStateTaxId = cust.ThirdStateTaxId?.ToUpper();

            await _customerRepository.AddCustomerAsync(cust);
            return new Response<Customer>(cust);
        }


    }
}
