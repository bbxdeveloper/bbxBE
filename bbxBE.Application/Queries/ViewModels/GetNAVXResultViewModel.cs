using bbxBE.Common.Attributes;
using System.ComponentModel;

namespace bbxBE.Application.Queries.ViewModels
{
    public class GetNAVXResultViewModel
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
        public string Value { get; set; }

        [ColumnLabel("Sor")]
        [Description("Sor")]
        public string Line { get; set; }

    }
}
