using bbxBE.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbxBE.Application.Interfaces
{
    public interface ICacheService<T>   where T : BaseEntity
    {
        public bool TryGetValue(long ID, out T value);
        public T AddOrUpdate(T value);
        public bool TryRemove(T value);
        public Task RefreshCacheAsynch(IQueryable<T> query);

    }
}
