using AutoMapper;
using AutoMapper.Configuration.Conventions;
using bbxBE.Application.BLL;
using bbxBE.Application.Commands.cmdImport;
using bbxBE.Application.Consts;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common;
using bbxBE.Common.Attributes;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using bbxBE.Domain.Settings;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Queries.qCustomer
{
    public class QueryTaxPayer : IRequest<Response<Customer>>
    {
        public string Taxnumber { get; set; }   

    }

    public class QueryTaxPayerFromNAVHandler : IRequestHandler<QueryTaxPayer, Response<Customer>>
    {
        private readonly ICustomerRepositoryAsync _CustomerRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        private readonly NAVSettings _NAVSettings;
        private readonly ILogger<QueryTaxPayer> _logger;

         
        public QueryTaxPayerFromNAVHandler(ICustomerRepositoryAsync customerRepository, IMapper mapper, IOptions<NAVSettings> NAVSettings, ILogger<QueryTaxPayer> logger, IConfiguration configuration)
        {
            _CustomerRepository = customerRepository;
            _mapper = mapper;
            _NAVSettings = NAVSettings.Value;
            _logger = logger;
            _configuration = configuration;

    }

        public async Task<Response<Customer>> Handle(QueryTaxPayer request, CancellationToken cancellationToken)
        {
            var bllNavObj = new bllNAV(_NAVSettings, _logger);
            var resTaxpayer = bllNavObj.QueryTaxPayer(request);
            var res = new Customer();
            if (resTaxpayer != null)
            {
                res.CustomerName = resTaxpayer.taxpayerName;
                //      res.CustomerBankAccountNumber = resTaxpayer.;
                res.CustomerVatStatus = CustomerVatStatusType.DOMESTIC.ToString();
                res.TaxpayerId = resTaxpayer.taxNumberDetail.taxpayerId;
                res.VatCode = resTaxpayer.taxNumberDetail.vatCode;
                res.CountyCode = resTaxpayer.taxNumberDetail.countyCode;
                res.ThirdStateTaxId = "";
                res.CountryCode = bbxBEConsts.CNTRY_HU;

                var addr = resTaxpayer.taxpayerAddressList.FirstOrDefault(f => f.taxpayerAddressType == TaxpayerAddressTypeType.HQ);
                if (addr == null)
                {
                    addr = resTaxpayer.taxpayerAddressList.FirstOrDefault();
                }
                if (addr != null)
                {
                 
                    res.Region = addr.taxpayerAddress.region;
                    res.PostalCode = addr.taxpayerAddress.postalCode;
                    res.City = addr.taxpayerAddress.city;
                    res.AdditionalAddressDetail  = String.Format( "{0} {1}", addr.taxpayerAddress.streetName , addr.taxpayerAddress.number);


                }


            }

            return new Response<Customer>(res);
        }

   


    }
}
