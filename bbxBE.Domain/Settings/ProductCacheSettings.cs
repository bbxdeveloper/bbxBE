using System;
using System.Collections.Generic;
using System.Text;

namespace bbxBE.Domain.Settings
{

    public class ProductCacheSettings
    {
        public int AbsoluteExpirationInHours { get; set; }
        public int SlidingExpirationInMinutes { get; set; }

    }
}
