using System.ComponentModel;

namespace bbxBE.Common.Enums
{
    public enum enNAVOperation
    {
        [Description("Nincs művelet")]
        NOOPERATION,
        [Description("Bizonylat beküldés")]
        MANAGEINVOICE,
        [Description("Bizonylat küldés visszavonása")]
        MANAGEANNULMENT
    }
}
