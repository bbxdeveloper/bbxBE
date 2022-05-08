using System;
using System.Collections.Generic;
using System.Text;

namespace bbxBE.Common.Locking
{
    public class LockHolder<T> : IDisposable where T : class
    {
        private T handle;
        private bool holdsLock;

        protected LockHolder(T handle, int milliSecondTimeout)
        {
            this.handle = handle;
            holdsLock = System.Threading.Monitor.TryEnter(
                handle, milliSecondTimeout);
        }

        protected LockHolder(T handle)
        {
            this.handle = handle;
            System.Threading.Monitor.Enter(handle);
            holdsLock = true;
        }


        public bool LockSuccessful
        {
            get { return holdsLock; }
        }

        #region IDisposable Members
        public void Dispose()
        {
            if (holdsLock)
                System.Threading.Monitor.Exit(handle);
            // Don’t unlock twice
            holdsLock = false;
        }
        #endregion
    }

}
