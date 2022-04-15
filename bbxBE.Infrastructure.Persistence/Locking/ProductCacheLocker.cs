using bbxBE.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace bbxBE.Infrastructure.Persistence.Locking
{

    public class ProductCacheLocker : LockHolder<object>
    {
        public ProductCacheLocker(object handle, int milliSecondTimeout)
         : base(handle, milliSecondTimeout)
        {

        }

        public ProductCacheLocker(object handle)
            : base(handle)
        {
        }
    }
}
