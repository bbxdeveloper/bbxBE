using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using System;

namespace bbxBE.Application.BLL
{

    public static class bllCounter
    {
        public static object Locker = new object();



        public static string GetCounterCode(enInvoiceType p_invoiceType, PaymentMethodType p_paymentMethod, bool Incoming, bool isCorrectionInvoice, long WarehouseID)
        {


            if (p_invoiceType == enInvoiceType.BLK)
            {
                //Blokk
                //

                var prefix = (p_paymentMethod == PaymentMethodType.CASH ? bbxBEConsts.DEF_BLKCOUNTER : bbxBEConsts.DEF_BLCCOUNTER);
                var whs = WarehouseID.ToString().PadLeft(3, '0');
                return String.Format($"{prefix}_{whs}");
            }
            else
            {
                if (!isCorrectionInvoice)
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
                    var first = bbxBEConsts.DEF_JSCOUNTER;
                    var second = (Incoming ? "B" : "K");
                    var third = WarehouseID.ToString().PadLeft(3, '0');
                    return String.Format($"{first}{second}_{third}");

                }
            }
        }


    }
}
