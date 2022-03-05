using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace bbxBE.Application.Enums
{
    public enum enUnitOfMeasure
    {
        [Description("DB")]
        PIECE,
//        [Description("Kilogram")]
//        KILOGRAM,
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
        [Description("Méter")]
        METER,
        [Description("Folyóméter")]
        LINEAR_METER,
//        [Description("Karton")]
//        CARTON,
        [Description("Doboz")]
        PACK,
        [Description("Egyéb")]
        OWN
    }
}
