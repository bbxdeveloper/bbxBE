using AutoMapper;
using bbxBE.Application.Interfaces.Repositories;
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
        public static bool ValidateIBAN(string IBAN)
        {
            IBAN = IBAN.ToUpper(); //IN ORDER TO COPE WITH THE REGEX BELOW
            if (String.IsNullOrEmpty(IBAN))
                return false;
            else if (System.Text.RegularExpressions.Regex.IsMatch(IBAN, "^[A-Z0-9]"))
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
                return checksum == 1;
            }
            else
                return false;
        }

        public static bool ValidateBankAccount(string bankAccount)
        {
            bankAccount = bankAccount.ToUpper(); //IN ORDER TO COPE WITH THE REGEX BELOW
            if (String.IsNullOrEmpty(bankAccount))
                return false;

            return (System.Text.RegularExpressions.Regex.IsMatch(bankAccount, "^[0-9]{8}-[0-9]{8}(-[0-9]{8})?$"));
  
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
