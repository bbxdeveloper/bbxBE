using bbxBE.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace bbxBE.Domain.Entities
{
    public class USR_USER : BaseEntity
    {
        public string USR_NAME { get; set; }
        
        public string USR_EMAIL { get; set; }
        public string USR_LOGIN { get; set; }
        public string USR_PASSWDHASH { get; set; }
        public string USR_COMMENT { get; set; }
        public bool USR_ACTIVE { get; set; }
        public bool DELETED { get; set; }

    }
}
