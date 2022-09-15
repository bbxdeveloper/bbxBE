using bbxBE.Domain.Common;
using System;
using System.Collections.Generic;
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
    }
}
