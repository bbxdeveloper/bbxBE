using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace bbxBE.Common.Enums
{
    public enum enStockCardTypes
    {
        [Description("Kezdőállapot")]
        INIT,
        [Description("Leltár")]
        INVC,                           //inventory control
        [Description("Foly.lelt")]
        INVC_C,                         ////inventory control continous
        [Description("Klt.diekt")]
        DIRECT,
        [Description("Szla/Száll")]
        INVOICE,
        [Description("Kiad")]
        TRANSFER,                       //csak a VALÓS készlet változik
        [Description("Kiad száml")]
        TRANSFERINV,                    //csak a SZÁMÍTOTT készlet változik
        [Description("SZNE")]
        SZNE                            //XXX-es

    }
}
