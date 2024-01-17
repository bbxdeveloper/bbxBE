using AutoMapper;
using bbxBE.Application.BLL;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Queries.qCustomer;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
using bbxBE.Common.Consts;
using bbxBE.Common.Exceptions;
using bbxBE.Domain.Entities;
using bbxBE.Domain.Settings;
using MediatR;
using Microsoft.Extensions.Options;
using Serilog;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace bxBE.Application.Commands.cmdCustomer
{
    public class UpdateCustomerCommand : IRequest<Response<Customer>>
    {
        [ColumnLabel("ID")]
        [Description("ID")]
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

        [ColumnLabel("Adóalany belföldi adószám")]
        [Description("Adóalany belföldi adószám")]
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

        [ColumnLabel("Legutoljára megadott kedvezmény %")]
        [Description("Legutoljára megadott bizonylatkedvezmény %")]
        public decimal? LatestDiscountPercent { get; set; }

        [ColumnLabel("Megjegyzés")]
        [Description("Megjegyzés")]
        public string Comment { get; set; }

        [ColumnLabel("Fordtott áfás?")]
        [Description("Fordtott áfás ?")]
        public bool IsFA { get; set; }


        [ColumnLabel("Saját adat?")]
        [Description("Saját adat? (csak egy ilyen rekord lehet)")]
        public bool IsOwnData { get; set; }

    }

    public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Response<Customer>>
    {
        private readonly ICustomerRepositoryAsync _customerRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly NAVSettings _NAVSettings;

        public UpdateCustomerCommandHandler(ICustomerRepositoryAsync customerRepository, IMapper mapper, IOptions<NAVSettings> NAVSettings, ILogger logger)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _NAVSettings = NAVSettings.Value;
            _logger = logger;
        }

        public async Task<Response<Customer>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            var cust = _mapper.Map<Customer>(request);
            cust.CustomerBankAccountNumber = cust.CustomerBankAccountNumber?.ToUpper();
            cust.ThirdStateTaxId = cust.ThirdStateTaxId?.ToUpper();

            if (!string.IsNullOrWhiteSpace(cust.TaxpayerId))
            {
                var bllNavObj = new bllNAV(_NAVSettings, _logger);
                var qt = new QueryTaxPayer() { Taxnumber = cust.TaxpayerId };
                var resTaxpayer = bllNavObj.QueryTaxPayer(qt);
                if (resTaxpayer == null)
                {
                    throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_CST_TAXNUMBER_INV3, cust.TaxpayerId));
                }
            }
            await _customerRepository.UpdateCustomerAsync(cust);
            return new Response<Customer>(cust);
        }


    }
}
