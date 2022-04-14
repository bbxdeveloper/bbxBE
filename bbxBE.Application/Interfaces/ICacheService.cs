using System;
using System.Collections.Generic;
using System.Text;

namespace bbxBE.Application.Interfaces
{
    public interface ICacheService
    {
        bool TryGet<T>(long ID, out T value);
        T Set<T>(long ID, T value);
        void Remove(long ID);
    }
}
