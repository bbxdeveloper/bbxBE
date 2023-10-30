using System.ComponentModel;

namespace bbxBE.Common.Enums
{
    public enum enUnitOfMeasure
    {
        [Description("DB")]
        PIECE,
        [Description("Kg")]
        KILOGRAM,
        //        [Description("Tonna")]
        //        TON,
        //        [Description("KWH")]
        //        KWH,
        //        [Description("Nap")]
        //        DAY,
        //        [Description("Óra")]
        //        HOUR,
        //        [Description("Perc")]
        //        MINUTE,
        //        [Description("Hónap")]
        //        MONTH,
        [Description("Liter")]
        LITER,
        //        [Description("Kilométer")]
        //        KILOMETER,
        //        [Description("Köbméter")]
        //        CUBIC_METER,
        [Description("m")]
        METER,
        [Description("FM")]
        LINEAR_METER,
        //        [Description("Karton")]
        //        CARTON,
        [Description("Doboz")]
        PACK,
        [Description("Egyéb")]
        OWN
    }
}
