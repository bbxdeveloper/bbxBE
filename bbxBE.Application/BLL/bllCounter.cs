using AutoMapper;
using bbxBE.Application.Interfaces.Repositories;
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

        public static async Task<string> SafeGetNextAsync(ICounterRepositoryAsync _CounterRepositoryAsync, string CounterCode, string WarehouseCode)
        {
            string num = "";
            string res = "???";
            try
            {
                num = await _CounterRepositoryAsync.GetNextValueAsync(CounterCode, WarehouseCode);
            }
            catch (Exception ex)
            {
                throw;
            }
            return res;
        }

   
    }
}
