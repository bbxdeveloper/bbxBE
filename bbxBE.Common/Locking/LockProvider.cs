using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Common.Locking
{

    //https://www.ryadel.com/en/asp-net-core-lock-threads-async-custom-ids-lockprovider/
    /// <summary>
    /// A LockProvider based upon the SemaphoreSlim class 
    /// to selectively lock objects, resources or statement blocks 
    /// according to given unique IDs in a sync or async way.
    /// </summary>
    public class LockProvider<T>
    {
        static readonly ConcurrentDictionary<T, SemaphoreSlim> lockDictionary =
            new ConcurrentDictionary<T, SemaphoreSlim>();

        public LockProvider() { }

        /// <summary>
        /// Blocks the current thread (according to the given ID)
        /// until it can enter the LockProvider
        /// </summary>
        /// <param name="idToLock">the unique ID to perform the lock</param>
        public void Wait(T idToLock)
        {
            lockDictionary.GetOrAdd(idToLock, new SemaphoreSlim(1, 1)).Wait();
        }

        /// <summary>
        /// Asynchronously puts thread to wait (according to the given ID)
        /// until it can enter the LockProvider
        /// </summary>
        /// <param name="idToLock">the unique ID to perform the lock</param>
        public async Task WaitAsync(T idToLock)
        {
            await lockDictionary.GetOrAdd(idToLock, new SemaphoreSlim(1, 1)).WaitAsync();
        }

        public async Task<bool> WaitAsync(T idToLock, int millisecondsTimeout)
        {
            return await lockDictionary.GetOrAdd(idToLock, new SemaphoreSlim(1, 1)).WaitAsync(millisecondsTimeout);
        }

        /// <summary>
        /// Releases the lock (according to the given ID)
        /// </summary>
        /// <param name="idToUnlock">the unique ID to unlock</param>
        public void Release(T idToUnlock)
        {
            SemaphoreSlim semaphore;
            if (lockDictionary.TryGetValue(idToUnlock, out semaphore))
                semaphore.Release();
        }
    }
}
