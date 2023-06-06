using System.ComponentModel;

namespace bbxBE.Common.Enums
{
    public enum enStockCardType
    {
        [Description("Kezdőállapot")]
        INIT,
        [Description("Leltár")]
        ICP,                           //inventory control
        [Description("Foly.lelt")]
        ICC,                           //inventory control continous 
        [Description("Klt.diekt módosítás")]
        DIRECT,
        [Description("Szla/Száll")]
        INV_DLV,
        [Description("Raktárközi kiadás")]
        WHSTRANSFER,
        [Description("SZNE")]
        SZNE                            //XXX-es

    }
}
