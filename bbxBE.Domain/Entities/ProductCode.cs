using bbxBE.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace bbxBE.Domain.Entities
{
    public class ProductCode : BaseEntity
    {
        public long ProductID { get; set; }
        public string ProductCodeCategory { get; set; }
        public string ProductCodeValue { get; set; }

    }
}
