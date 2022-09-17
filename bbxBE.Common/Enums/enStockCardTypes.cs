using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace bbxBE.Common.Enums
{
    public enum enStockCardType
    {
        [Description("Kezdőállapot")]
        INIT,
        [Description("Leltár")]         
        ICP,                           //inventory control
        [Description("Foly.lelt")]  
        ICC,                           //inventory control continous (egyelőre nics megvalósítva)
        [Description("Klt.diekt")]
        DIRECT,
        [Description("Szla/Száll")]
        INV_DLV,
        [Description("Kiad")]
        TRANSFER,                       //csak a VALÓS készlet változik
        [Description("Kiad száml")]
        TRANSFERINV,                    //csak a SZÁMÍTOTT készlet változik
        [Description("SZNE")]
        SZNE                            //XXX-es

    }
}
