using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bbxBE.Application.Interfaces
{
    public interface IGenericRepositoryAsync<T> where T : class
    {
        Task<T> GetByIdAsync(long ID);

        Task<IEnumerable<T>> GetAllAsync();

        Task<IEnumerable<T>> GetPagedReponseAsync(int pageNumber, int pageSize);

        Task<IEnumerable<T>> GetPagedAdvancedReponseAsync(int pageNumber, int pageSize, string orderBy, string fields);

        Task<T> AddAsync(T entity, bool savechenges = true);

        Task UpdateAsync(T entity, bool savechenges = true);

        Task RemoveAsync(T entity, bool savechenges = true);

        Task AddRangeAsync(IEnumerable<T> entities, bool savechenges = true);

        Task UpdateRangeAsync(IEnumerable<T> entities, bool savechenges = true);

        Task RemoveRangeAsync(IEnumerable<T> entities,bool savechenges = true);

    }
}