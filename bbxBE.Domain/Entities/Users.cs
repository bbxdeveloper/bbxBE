using bbxBE.Common.Attributes;
using bbxBE.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;

namespace bbxBE.Domain.Entities
{
    [DataContract]
    public class Users : BaseEntity
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string LoginName { get; set; }

        [IgnoreDataMemberAttribute]
        [JsonIgnoreAttribute]
        public string PasswordHash { get; set; }

        [DataMember]
        public string Comment { get; set; }

        [DataMember]
        public bool Active { get; set; }


        [ColumnLabel("Számlák/Szállítólevele")]
        [Description("Számlák/Szállítólevele")]
        public virtual List<Invoice> InvoiceList { get; set; }

        [ColumnLabel("Árajánlat")]
        [Description("Árajánlat")]
        public virtual List<Offer> OfferList { get; set; }

    }
}
