using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace bbxBE.Common.Enums
{
    public enum enInvoiceType
    {
        [Description("Számla")]
        INV,
        [Description("Kimenő szállítólevél")]
        DNO,
        [Description("Bevételezés")]
        INC,
        [Description("Bejövő szállítólevél")]
        DNI,
        [Description("Blokk")]
        BLK
    }
}
