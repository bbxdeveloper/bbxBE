using bbxBE.Common.Attributes;
using bbxBE.Domain.Common;
using System.ComponentModel;

namespace bbxBE.Domain.Entities
{
    [Description("NAV adatcsere eredmény")]
    public class NAVXResult : BaseEntity
    {

        [ColumnLabel("NAVXChange ID")]
        [Description("NAVXChange ID")]
        public long NAVXChangeID { get; set; }

        [ColumnLabel("Eredmény kód")]
        [Description("Eredmény kód")]
        public string ResultCode { get; set; }

        [ColumnLabel("Hibakód")]
        [Description("Hibakód")]
        public string ErrorCode { get; set; }

        [ColumnLabel("Üzenet")]
        [Description("Üzenet")]
        public string Message { get; set; }

        [ColumnLabel("Tag")]
        [Description("Tag")]
        public string Tag { get; set; }

        [ColumnLabel("Érték")]
        [Description("Érték")]
        public string Érték { get; set; }

        [ColumnLabel("Sor")]
        [Description("Sor")]
        public string Line { get; set; }

        //navigációs komponens
        [ColumnLabel("NAV adatcsere")]
        [Description("NAV adatcsere")]
        public virtual NAVXChange XChange { get; set; }

    }
}
