using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using System;
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
            catch (Exception)
            {
                throw;
            }
            return res;
        }

        public static string GetCounterCode(enInvoiceType p_invoiceType, PaymentMethodType p_paymentMethod, bool Incoming, long? OriginalInvoiceID, long WarehouseID)
        {


            if (p_invoiceType == enInvoiceType.BLK)
            {
                //Blokk
                //

                var prefix = (p_paymentMethod == PaymentMethodType.CASH ? "BLK" : "BLC");
                var whs = WarehouseID.ToString().PadLeft(3, '0');
                return String.Format($"{prefix}_{whs}");
            }
            else
            {
                if (!OriginalInvoiceID.HasValue || OriginalInvoiceID.Value == 0)
                {
                    //NORMÁL számla, szállítólevél
                    //
                    var first = (Incoming ? "B" : "K");
                    var second = p_invoiceType.ToString();
                    var third = WarehouseID.ToString().PadLeft(3, '0');
                    return String.Format($"{first}{second}_{third}");
                }
                else
                {
                    //Javítószámla
                    //
                    var first = "JS";
                    var second = (Incoming ? "B" : "K");
                    var third = WarehouseID.ToString().PadLeft(3, '0');
                    return String.Format($"{first}{second}_{third}");

                }
            }
        }


    }
}
