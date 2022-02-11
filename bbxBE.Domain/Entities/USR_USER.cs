using bbxBE.Domain.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;

namespace bbxBE.Domain.Entities
{
    [DataContract]
    public class USR_USER : BaseEntity
    {
        [DataMember]
        public string USR_NAME { get; set; }

        [DataMember]
        public string USR_EMAIL { get; set; }
        [DataMember]
        public string USR_LOGIN { get; set; }

        [JsonIgnore]
        public string USR_PASSWDHASH { get; set; }
        [DataMember]
        public string USR_COMMENT { get; set; }
        [DataMember]
        public bool USR_ACTIVE { get; set; }
        [DataMember]
        public bool DELETED { get; set; }

    }
}
