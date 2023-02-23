using System;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace bbxBE.Common.Enums
{
    public enum enValidateBankAccountResult
    {
        [Description("OK")] 
        OK,
        [Description("Nincs kitöltve")]
        ERR_EMPTY,
        [Description("Helytelen formátum")]
        ERR_FORMAT,
        [Description("Helytelen tartalom")]
        ERR_CHECKSUM

    }
}
