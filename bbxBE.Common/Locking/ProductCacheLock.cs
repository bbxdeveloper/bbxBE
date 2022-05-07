using System;
using System.Collections.Generic;
using System.Text;

namespace bbxBE.Common.Locking
{
    public class ProductCacheLock : LockHolder<object>
    {
        public ProductCacheLock(object handle, int milliSecondTimeout)
         : base(handle, milliSecondTimeout)
        {

        }

        public ProductCacheLock(object handle)
            : base(handle)
        {
        }
    }

}
