using System.ComponentModel;

namespace bbxBE.Common.Enums
{
    public enum enInvoiceType
    {
        [Description("Kimenő számla")]
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
