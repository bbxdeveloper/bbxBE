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


            var prefix = "";
            if (p_invoiceType == enInvoiceType.BLK)
            {
                //Blokk
                //

                prefix = (p_paymentMethod == PaymentMethodType.CASH ? bbxBEConsts.DEF_BLKCOUNTER : bbxBEConsts.DEF_BLCCOUNTER);
            }
            else
            {
                if (!isCorrectionInvoice)
                {
                    //NORMÁL számla, szállítólevél
                    //
                    if (Incoming)
                    {
                        if (p_invoiceType == enInvoiceType.DNI)
                        {
                            prefix = bbxBEConsts.DEF_BESCOUNTER;

                        }
                        else
                        {
                            prefix = bbxBEConsts.DEF_BEVCOUNTER;
                        }

                    }
                    else
                    {
                        switch (p_paymentMethod)
                        {
                            case PaymentMethodType.TRANSFER:
                                prefix = bbxBEConsts.DEF_ACOUNTER;
                                break;
                            case PaymentMethodType.CASH:
                                prefix = bbxBEConsts.DEF_KCOUNTER;
                                break;
                            case PaymentMethodType.CARD:
                                prefix = bbxBEConsts.DEF_CCOUNTER;
                                break;
                            case PaymentMethodType.VOUCHER:
                                prefix = bbxBEConsts.DEF_KCOUNTER;
                                break;
                            case PaymentMethodType.OTHER:
                                prefix = bbxBEConsts.DEF_KCOUNTER;
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    if (Incoming)
                    {
                        prefix = bbxBEConsts.DEF_BEJCOUNTER;

                    }
                    else
                    {
                        prefix = bbxBEConsts.DEF_JAVCOUNTER;
                    }
                }

            }
            var whs = WarehouseID.ToString().PadLeft(3, '0');
            return String.Format($"{prefix}_{whs}");
        }
    }
}