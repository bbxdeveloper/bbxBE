using bbxBE.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace bbxBE.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string ProductGroupCode { get; set; }
        public string ProductGroupDescription { get; set; }
    }
}
