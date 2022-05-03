using AutoMapper;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common.Enums;
using bbxBE.Domain.Entities;
using bxBE.Application.Commands.cmdCounter;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.BLL
{

    public static class bllCounter
    {
        public static object Locker = new object();

        public static async Task<string> SafeGetNextAsync(ICounterRepositoryAsync _CounterRepositoryAsync, string CounterCode, long WarehouseID)
        {
            string num = "";
            string res = "???";
            try
            {
                num = await _CounterRepositoryAsync.GetNextValueAsync(CounterCode, WarehouseID);
            }
            catch (Exception ex)
            {
                throw;
            }
            return res;
        }

        public static string GetCounterCode(enInvoiceType p_nvoiceType, bool Incoming, long WarehouseID)
        {
            var first = (Incoming ? "B" : "K");
            var second = p_nvoiceType.ToString();
            var third = WarehouseID.ToString().PadLeft(3, '0');
            return String.Format($"{first}{second}_{third}");
        }


    }
}
