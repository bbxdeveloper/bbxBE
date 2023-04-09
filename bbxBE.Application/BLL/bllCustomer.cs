using AutoMapper;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common.Enums;
using bbxBE.Domain.Entities;
using bxBE.Application.Commands.cmdCustomer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.BLL
{
    public static class bllCustomer
    {
        public static enValidateBankAccountResult ValidateIBAN(string IBAN)
        {
            IBAN = IBAN.ToUpper(); //IN ORDER TO COPE WITH THE REGEX BELOW
            if (String.IsNullOrEmpty(IBAN))
                return enValidateBankAccountResult.ERR_EMPTY;

            //https://www.regextester.com/115565
            //https://en.wikipedia.org/wiki/International_Bank_Account_Number
            else if (System.Text.RegularExpressions.Regex.IsMatch(IBAN, @"^(?:(?:IT|SM)\d{2}[A-Z]\d{22}|CY\d{2}[A-Z]\d{23}|NL\d{2}[A-Z]{4}\d{10}|LV\d{2}[A-Z]{4}\d{13}|(?:BG|BH|GB|IE)\d{2}[A-Z]{4}\d{14}|GI\d{2}[A-Z]{4}\d{15}|RO\d{2}[A-Z]{4}\d{16}|KW\d{2}[A-Z]{4}\d{22}|MT\d{2}[A-Z]{4}\d{23}|NO\d{13}|(?:DK|FI|GL|FO)\d{16}|MK\d{17}|(?:AT|EE|KZ|LU|XK)\d{18}|(?:BA|HR|LI|CH|CR)\d{19}|(?:GE|DE|LT|ME|RS)\d{20}|IL\d{21}|(?:AD|CZ|ES|MD|SA)\d{22}|PT\d{23}|(?:BE|IS)\d{24}|(?:FR|MR|MC)\d{25}|(?:AL|DO|LB|PL)\d{26}|(?:AZ|HU)\d{26}|(?:GR|MU)\d{28})$"))
            {
                IBAN = IBAN.Replace(" ", String.Empty);
                string bank =
                IBAN.Substring(4, IBAN.Length - 4) + IBAN.Substring(0, 4);
                int asciiShift = 55;
                StringBuilder sb = new StringBuilder();
                foreach (char c in bank)
                {
                    int v;
                    if (Char.IsLetter(c)) v = c - asciiShift;
                    else v = int.Parse(c.ToString());
                    sb.Append(v);
                }
                string checkSumString = sb.ToString();
                int checksum = int.Parse(checkSumString.Substring(0, 1));
                for (int i = 1; i < checkSumString.Length; i++)
                {
                    int v = int.Parse(checkSumString.Substring(i, 1));
                    checksum *= 10;
                    checksum += v;
                    checksum %= 97;
                }
                if (checksum == 1)
                    return enValidateBankAccountResult.OK;
                else
                    return enValidateBankAccountResult.ERR_CHECKSUM;
            }
            else
                return enValidateBankAccountResult.ERR_FORMAT;
        }

        public static enValidateBankAccountResult ValidateBankAccount(string bankAccount)
        {
            bankAccount = bankAccount.ToUpper(); //IN ORDER TO COPE WITH THE REGEX BELOW
            if (String.IsNullOrEmpty(bankAccount))
                return enValidateBankAccountResult.ERR_EMPTY;

            return (System.Text.RegularExpressions.Regex.IsMatch(bankAccount, "^[0-9]{8}-[0-9]{8}(-[0-9]{8})?$") ? enValidateBankAccountResult.OK : enValidateBankAccountResult.ERR_FORMAT);
        }

        public static bool ValidateTaxPayerNumber(string taxPayerNumber)
        {
            return (System.Text.RegularExpressions.Regex.IsMatch(taxPayerNumber, @"^(\d{7})(\d)\-([1-5])\-(0[2-9]|[13][0-9]|2[02-9]|4[0-4]|51)$"));
        }

        public static bool ValidateCountryCode(string countryCode)
        {
            var valid = Enum.TryParse(countryCode, out enCountries cou);
            return valid;
        }
        public static bool ValidateUnitPriceType(string unitPriceType)
        {
            var valid = Enum.TryParse(unitPriceType, out enUnitPriceType upt);
            return valid;
        }


        public static async Task<int> CreateRangeAsynch(List<CreateCustomerCommand> requestList,
             ICustomerRepositoryAsync _CustomerRepository, IMapper _mapper, CancellationToken cancellationToken)
        {
            var customerList = new List<Customer>();
            foreach (var customer in requestList)
            {
                var cust = _mapper.Map<Customer>(customer);
                customerList.Add(cust);
            }

            return await _CustomerRepository.AddCustomerRangeAsync(customerList);
        }

    }
}
