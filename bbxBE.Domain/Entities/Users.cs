using bbxBE.Common.Attributes;
using bbxBE.Domain.Common;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace bbxBE.Domain.Entities
{
    [DataContract]
    public class Users : BaseEntity
    {
        [DataMember]
        [ColumnLabel("Név")]
        [Description("Név")]
        public string Name { get; set; }

        [DataMember]
        [ColumnLabel("E-mail")]
        [Description("E-mail")]
        public string Email { get; set; }

        [DataMember]
        [ColumnLabel("Login név")]
        [Description("Login név")]
        public string LoginName { get; set; }

        [IgnoreDataMemberAttribute]
        [JsonIgnoreAttribute]
        [ColumnLabel("Jelszó HASH")]
        [Description("Jelszó HASH")]
        public string PasswordHash { get; set; }

        [DataMember]
        [ColumnLabel("Megjegyzés")]
        [Description("Megjegyzés")]
        public string Comment { get; set; }

        [DataMember]
        [ColumnLabel("Aktív?")]
        [Description("Aktív?")]
        public bool Active { get; set; }

        /*
        [ColumnLabel("Számlák/Szállítólevelek")]
        [Description("Számlák/Szállítólevelek")]
        public virtual List<Invoice> InvoiceList { get; set; }

        [ColumnLabel("Árajánlat")]
        [Description("Árajánlat")]
        public virtual List<Offer> OfferList { get; set; }
        */
    }
}
