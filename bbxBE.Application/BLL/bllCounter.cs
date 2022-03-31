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
                num = await _CounterRepositoryAsync.GetNextAsync(CounterCode, WarehouseCode);
            }
            catch (Exception ex)
            {
         //       NumberGenerator.Instance.FinalizeNumber(p_code, Int32.Parse(num), true);
                throw;
            }
            return res;
        }

        /*
        private const int ExpiredInMinutes = 30;
        /// <summary>
        /// Vissazadja a kódhoz tartozo kovetkezo sorszamot
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        private string GetNext(string p_code, string p_sessionID, string p_user)
        {
            int retNumber = 0;
            var bllLog = new BllReportCenterLogs();
            try
            {
                using (LockForNumberGenerator lockObj = new LockForNumberGenerator(Locker))
                {

                    p_code = p_code.ToUpper();
                    BllNumbers bllNumber = new BllNumbers();
                    var number = bllNumber.Retrieve(p_code);
                    if (number == null)
                    {
                        number = new Numbers() { Code = p_code };
                        number.Number = 0;
                    }

                    //teszthez:number.UsedNumberList.OrderBy( o=>o.TimeStamp).Select(s=>s.Number.ToString() + "-->" +  s.TimeStamp.ToString()).ToList()
                    //

                    Numbers.CUsedNumber oldestExpired = (number.UsedNumberList != null ? number.UsedNumberList.Where(w => DateTime.UtcNow.Ticks - w.Ticks > TimeSpan.TicksPerMinute * ExpiredInMinutes)
                                        .OrderBy(o => o.Number).FirstOrDefault() : null);
                    if (oldestExpired == null)
                    {
                        number.Number += 1;

                        number.UsedNumberList.Add(new Numbers.CUsedNumber()
                        { Number = number.Number, Ticks = DateTime.UtcNow.Ticks, SessionID = p_sessionID, User = p_user });
                        retNumber = number.Number;

                        bllLog.WriteLog(AzureTableStore.Instance.NextID(), DateTime.UtcNow.Ticks, "NumberGenerator", "INFO", "", DateTime.UtcNow,
                            "GetNext", $"Use number:{p_code}{ retNumber:D4}");
                    }
                    else
                    {
                        retNumber = oldestExpired.Number;
                        bllLog.WriteLog(AzureTableStore.Instance.NextID(), DateTime.UtcNow.Ticks, "NumberGenerator", "INFO", "", DateTime.UtcNow,
                            "GetNext", $"Use expired number:{p_code}{ retNumber:D4}, last used:" + new DateTime(oldestExpired.Ticks).ToString());

                        oldestExpired.Ticks = DateTime.UtcNow.Ticks;
                        oldestExpired.SessionID = p_sessionID;
                        number.State = AzureTableObjBase.enObjectState.Modified;

                    }
                    bllNumber.MaintainItem(number);

                    return $"{retNumber:D4}";
                }
            }
            catch (Exception ex)
            {
                bllLog.WriteLog(AzureTableStore.Instance.NextID(), DateTime.UtcNow.Ticks, "NumberGenerator", "ERROR", "", DateTime.UtcNow,
                    "GetNext", $"Number:{p_code}{ retNumber:D4}, exception:" + ex.Message);
                FinalizeNumber(p_code, retNumber, true);
                ExceptionDispatchInfo.Capture(ex).Throw();
                throw;
            }
        }
        */
    }
}
