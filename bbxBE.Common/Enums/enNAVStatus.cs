using System.ComponentModel;

namespace bbxBE.Common.Enums
{
    public enum enNAVStatus
    {
        [Description("Létrehozva")]
        CREATED,
        [Description("Token elküldve")]
        TOKEN_SENT,
        [Description("Bizonylat/visszavonás elküldve")]
        DATA_SENT,
        [Description("Rendben")]
        DONE,
        [Description("Hiba")]
        ERROR,
        [Description("NAV hiba")]
        NAV_ERROR,
        [Description("Megszakítva")]
        ABORTED,
        [Description("Ismeretlen állapot")]
        UNKNOWN
    }
}
