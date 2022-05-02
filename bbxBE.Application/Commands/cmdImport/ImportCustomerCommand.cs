using AutoMapper;
using bbxBE.Application.BLL;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Domain.Entities;
using bbxBE.Domain.Extensions;
using bxBE.Application.Commands.cmdCustomer;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Commands.cmdImport
{
    public class ImportCustomerCommand : IRequest<Response<ImportedItemsStatistics>>
    {
        public List<IFormFile> CustomerFiles { get; set; }
        public string FieldSeparator { get; set; } = ";";
    }

    public class ImportCustomerCommandHandler : CustomerMappingParser, IRequestHandler<ImportCustomerCommand, Response<ImportedItemsStatistics>>
    {
        private const string CustomerNameFieldName = "CustomerName";
        private const string CustomerName2FieldName = "VEVONEV2";
        private const string PostalCodeFieldName = "PostalCode";
        private const string CityFieldName = "City";
        private const string CommentFieldName = "Comment";
        private const string CountryCodeFieldName = "CountryCode";
        private const string ThirdStateTaxIdFieldName = "ThirdStateTaxId";
        private const string AdditionalAddressDetailFieldName = "AdditionalAddressDetail";
        private const string HouseNumberFieldName = "HouseNumber";
        private const string CustomerBankAccountNumberFieldName = "CustomerBankAccountNumber";
        private const string TaxpayerIdFieldName = "TaxpayerId";

        private readonly ICustomerRepositoryAsync _customerRepository;
        //private readonly IUnitOfMEasure
        private readonly IMapper _mapper;
        private readonly ILogger _logger;



        public ImportCustomerCommandHandler(ICustomerRepositoryAsync customerRepository,

                                            IMapper mapper,
                                            ILogger<ImportProductCommandHandler> logger)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Response<ImportedItemsStatistics>> Handle(ImportCustomerCommand request, CancellationToken cancellationToken)
        {
            var mappedCustomerColumns = new CustomerMappingParser().GetCustomerMapping(request).ReCalculateIndexValues();
            var customerItemsFromCSV = await GetCustomerItemsAsync(request, mappedCustomerColumns.customerMap);
            var importCustomerResponse = new ImportedItemsStatistics { AllItemsCount = customerItemsFromCSV.Count };

            await bllCustomer.CreateRangeAsynch(customerItemsFromCSV, _customerRepository, _mapper, cancellationToken);
            return new Response<ImportedItemsStatistics>(importCustomerResponse);
        }

        private static async Task<List<CreateCustomerCommand>> GetCustomerItemsAsync(ImportCustomerCommand request, Dictionary<string, int> customerMapping)
        {
            var customerItems = new List<CreateCustomerCommand>();
            using (var reader = new StreamReader(request.CustomerFiles[1].OpenReadStream()))
            {
                string currentLine;
                while ((currentLine = await reader.ReadLineAsync()) != null)
                {
                    customerItems.Add(GetCustomerFromCSV(currentLine, customerMapping, request.FieldSeparator));
                }
            }

            return customerItems;
        }

        private static CreateCustomerCommand GetCustomerFromCSV(string currentLine, Dictionary<string, int> customerMapping, string fieldSeparator)
        {
            string regExpPattern = $"{fieldSeparator}(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))";
            Regex regexp = new Regex(regExpPattern);
            string[] currentFieldsArray = regexp.Split(currentLine);


            string regExpPatternForBankAccount = $"([0-9]{{8}}-[0-9]{{8}}-[0-9]{{8}}-[0-9]{{8}}|[0-9]{{8}}-[0-9]{{8}}-[0-9]{{8}})";
            string regExpPatternForVatNumber = $"[0-9]{{8}}-[0-9]{{1}}-[0-9]{{2}}";
            var createCustomerCommand = new CreateCustomerCommand();

            try
            {
                var custName1 = customerMapping.ContainsKey(CustomerNameFieldName) ? currentFieldsArray[customerMapping[CustomerNameFieldName]].Replace("\"", "").Trim() : null;
                var custName2 = customerMapping.ContainsKey(CustomerName2FieldName) ? currentFieldsArray[customerMapping[CustomerName2FieldName]].Replace("\"", "").Trim() : null;
                createCustomerCommand.CustomerName = !string.IsNullOrEmpty(custName1) || !string.IsNullOrEmpty(custName2) ? (custName1 + " " + custName2) : null;

                createCustomerCommand.PostalCode = customerMapping.ContainsKey(PostalCodeFieldName) ? currentFieldsArray[customerMapping[PostalCodeFieldName]].Replace("\"", "").Trim() : null;
                createCustomerCommand.City = customerMapping.ContainsKey(CityFieldName) ? currentFieldsArray[customerMapping[CityFieldName]].Replace("\"", "").Trim() : null;
                createCustomerCommand.Comment = customerMapping.ContainsKey(CommentFieldName) ? currentFieldsArray[customerMapping[CommentFieldName]].Replace("\"", "").Trim() : null;
                createCustomerCommand.ThirdStateTaxId = customerMapping.ContainsKey(ThirdStateTaxIdFieldName) ? currentFieldsArray[customerMapping[ThirdStateTaxIdFieldName]].Replace("\"", "").Trim() : null;

                var AdditionalAddressDetail = customerMapping.ContainsKey(AdditionalAddressDetailFieldName) ? currentFieldsArray[customerMapping[AdditionalAddressDetailFieldName]].Replace("\"", "").Trim() : null;
                var HouseNumber = customerMapping.ContainsKey(HouseNumberFieldName) ? currentFieldsArray[customerMapping[HouseNumberFieldName]].Replace("\"", "").Trim() : null;
                createCustomerCommand.AdditionalAddressDetail = (!string.IsNullOrEmpty(AdditionalAddressDetail)) || (!string.IsNullOrEmpty(HouseNumber)) ? (AdditionalAddressDetail + " " + HouseNumber) : String.Empty;


                var bankAccountNumber = customerMapping.ContainsKey(CustomerBankAccountNumberFieldName)
                        ? currentFieldsArray[customerMapping[CustomerBankAccountNumberFieldName]].Replace("\"", "").Trim() : null;
                if ((!string.IsNullOrEmpty(bankAccountNumber)) &&
                    (new Regex(regExpPatternForBankAccount).Match(bankAccountNumber).Success))
                    createCustomerCommand.CustomerBankAccountNumber = bankAccountNumber;

                var vatnumber = customerMapping.ContainsKey(TaxpayerIdFieldName) ? currentFieldsArray[customerMapping[TaxpayerIdFieldName]].Replace("\"", "").Trim() : null;
                if ((!string.IsNullOrEmpty(vatnumber)) && (new Regex(regExpPatternForVatNumber).Match(vatnumber).Success))
                {
                    createCustomerCommand.TaxpayerNumber = vatnumber.Substring(0, 6);
                    createCustomerCommand.VatCode = vatnumber.Substring(7, 1);
                    createCustomerCommand.CountryCode = vatnumber.Substring(8, 2);
                }
                if (string.IsNullOrEmpty(createCustomerCommand.CountryCode))
                    createCustomerCommand.CountryCode = "HU";

                return createCustomerCommand;
            }
            catch (System.Exception ex)
            {
                throw new Exception($"{ex.Message} - customerName: {currentFieldsArray[customerMapping[CustomerNameFieldName]]}");
            }
        }
    }
}
