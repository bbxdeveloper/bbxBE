using System.ComponentModel;

namespace bbxBE.Common.Enums
{
    public enum enNAVStatus
    {
        [Description("Létrehozva")]
        CREATED,
        [Description("Elküldött")]
        SENT,
        [Description("Rendben")]
        DONE,
        [Description("Hiba")]
        ERROR,
        [Description("Megszakítva")]
        ABORTED,
        [Description("Ismeretlen állapot")]
        UNKNOWN
    }
}
