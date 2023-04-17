using Microsoft.EntityFrameworkCore;
using System;

namespace bbxBE.Application.Interfaces
{
    public interface IDbContext : IDisposable
    {
        DbContext Instance { get; }
    }

}
