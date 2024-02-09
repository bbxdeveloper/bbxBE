using AutoMapper;
using bbxBE.Application.Commands.ResultModels;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using bxBE.Application.Commands.cmdCustomer;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Commands.cmdImport
{
    public class ImportCustomerCommand : IRequest<Response<ImportedItemsStatistics>>
    {
        [ColumnLabel("Importfájlok")]
        [Description("Importfájlok: 1.Mapper, 2.CSV")]
        public List<IFormFile> CustomerFiles { get; set; }

        [ColumnLabel("Mezőelválasztó")]
        [Description("Mezőelválasztó")]
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
        private const string CustomerUnitPriceTypeFieldName = "V_ARTIP";
        private const string CustomerDefPaymentMethod = "V_FIZM";
        private const string CustomerPaymentDaysFieldName = "V_FIZH";
        private const string CustomerLatestDiscountPercentFieldName = "V_ENG";
        private const string CustomerMaxLimitFieldName = "LIMIT";
        private const string CustomerIsFAFieldName = "V_FAFA";
        private const string CustomerPrivatePersonFieldName = "MAGAN";

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

            var customerList = new List<Customer>();
            foreach (var customer in customerItemsFromCSV)
            {
                var cust = _mapper.Map<Customer>(customer);
                customerList.Add(cust);
            }

            await _customerRepository.AddCustomerRangeAsync(customerList);

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
            string[] currentFieldsArray = regexp.Split(currentLine.Replace("\r", ""));


            string regExpPatternForBankAccount = $"([0-9]{{8}}-[0-9]{{8}}-[0-9]{{8}}-[0-9]{{8}}|[0-9]{{8}}-[0-9]{{8}}-[0-9]{{8}})";
            string regExpPatternForVatNumber = $"[0-9]{{8}}-[0-9]{{1}}-[0-9]{{2}}";
            var createCustomerCommand = new CreateCustomerCommand();

            try
            {
                var custName1 = customerMapping.ContainsKey(CustomerNameFieldName) ? currentFieldsArray[customerMapping[CustomerNameFieldName]].Replace("\"", "").Trim() : null;
                var custName2 = customerMapping.ContainsKey(CustomerName2FieldName) ? currentFieldsArray[customerMapping[CustomerName2FieldName]].Replace("\"", "").Trim() : null;
                createCustomerCommand.CustomerName = !string.IsNullOrEmpty(custName1) || !string.IsNullOrEmpty(custName2) ? (custName1 + " " + custName2).Trim() : null;

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
                    (Regex.IsMatch(bankAccountNumber, regExpPatternForBankAccount)))
                    createCustomerCommand.CustomerBankAccountNumber = bankAccountNumber;

                var vatnumber = customerMapping.ContainsKey(TaxpayerIdFieldName) ? currentFieldsArray[customerMapping[TaxpayerIdFieldName]].Replace("\"", "").Trim() : null;
                if ((!string.IsNullOrEmpty(vatnumber)) && (Regex.IsMatch(vatnumber, regExpPatternForVatNumber)))
                {
                    createCustomerCommand.TaxpayerNumber = vatnumber;
                }
                if (string.IsNullOrEmpty(createCustomerCommand.CountryCode))
                    createCustomerCommand.CountryCode = "HU";

                var unitPriceType = customerMapping.ContainsKey(CustomerUnitPriceTypeFieldName) ? currentFieldsArray[customerMapping[CustomerUnitPriceTypeFieldName]].Replace("\"", "").Trim() : null;
                createCustomerCommand.UnitPriceType = unitPriceType.Equals("1") ? "UNIT" : "LIST";

                var defPaymentMethod = customerMapping.ContainsKey(CustomerDefPaymentMethod) ? currentFieldsArray[customerMapping[CustomerDefPaymentMethod]].Replace("\"", "").Trim() : null;
                createCustomerCommand.DefPaymentMethod = defPaymentMethod.Equals("1") ? PaymentMethodType.CASH.ToString()
                    : defPaymentMethod.Equals("2") ? PaymentMethodType.TRANSFER.ToString() : PaymentMethodType.CASH.ToString();

                if (customerMapping.ContainsKey(CustomerLatestDiscountPercentFieldName)
                    && Decimal.TryParse(currentFieldsArray[customerMapping[CustomerLatestDiscountPercentFieldName]], out decimal latestDiscountPercent))
                {
                    createCustomerCommand.LatestDiscountPercent = latestDiscountPercent;
                }
                else
                {
                    createCustomerCommand.LatestDiscountPercent = (decimal?)null;
                }

                if (customerMapping.ContainsKey(CustomerPaymentDaysFieldName)
                    && Int16.TryParse(currentFieldsArray[customerMapping[CustomerPaymentDaysFieldName]], out short paymentDays))
                {
                    createCustomerCommand.PaymentDays = paymentDays;
                }
                else
                {
                    createCustomerCommand.PaymentDays = 0;
                }

                if (customerMapping.ContainsKey(CustomerMaxLimitFieldName)
                    && Decimal.TryParse(currentFieldsArray[customerMapping[CustomerMaxLimitFieldName]], out decimal maxLimit))
                {
                    createCustomerCommand.MaxLimit = maxLimit;
                }
                else
                {
                    createCustomerCommand.MaxLimit = (decimal?)null;
                }

                if (customerMapping.ContainsKey(CustomerIsFAFieldName))
                {
                    var isFA =
                        currentFieldsArray[customerMapping[CustomerIsFAFieldName]] == "1"
                        || currentFieldsArray[customerMapping[CustomerIsFAFieldName]] == "IGAZ"
                        || currentFieldsArray[customerMapping[CustomerIsFAFieldName]] == "TRUE";

                    createCustomerCommand.IsFA = isFA;
                }
                else
                {
                    createCustomerCommand.IsFA = false;
                }


                if (customerMapping.ContainsKey(CustomerPrivatePersonFieldName))
                {
                    var privatePerson =
                        currentFieldsArray[customerMapping[CustomerPrivatePersonFieldName]] == "1"
                        || currentFieldsArray[customerMapping[CustomerPrivatePersonFieldName]] == "IGAZ"
                        || currentFieldsArray[customerMapping[CustomerPrivatePersonFieldName]] == "TRUE";

                    createCustomerCommand.PrivatePerson = privatePerson;
                }
                else
                {
                    createCustomerCommand.PrivatePerson = false;
                }


                return createCustomerCommand;
            }
            catch (System.Exception ex)
            {
                throw new Exception($"{ex.Message} - customerName: {currentFieldsArray[customerMapping[CustomerNameFieldName]]}");
            }
        }
    }
}
