using Microsoft.EntityFrameworkCore;
using bbxBE.Application.Interfaces;
using bbxBE.Infrastructure.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using bbxBE.Common.Consts;
using static bbxBE.Common.NAV.NAV_enums;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using bbxBE.Domain.Common;

namespace bbxBE.Infrastructure.Persistence.Repository
{
    public class GenericRepositoryAsync<T> : IGenericRepositoryAsync<T> where T : BaseEntity

    {
        private readonly ApplicationDbContext _dbContext;

        public GenericRepositoryAsync(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public virtual async Task<T> GetByIdAsync(long ID)
        {
            return await _dbContext.Set<T>().FindAsync(ID);
        }

        public async Task<IEnumerable<T>> GetPagedReponseAsync(int pageNumber, int pageSize)
        {
            return await _dbContext
                .Set<T>()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<T>> GetPagedAdvancedReponseAsync(int pageNumber, int pageSize, string orderBy, string fields)
        {
            return await _dbContext
                .Set<T>()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select<T>("new(" + fields + ")")
                .OrderBy(orderBy)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<T> AddAsync(T entity, bool savechenges = true)
        {
            await _dbContext.Set<T>().AddAsync(entity);
            if (savechenges)
                await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(T entity, bool savechenges = true)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            if (savechenges)
                await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveAsync(T entity, bool savechenges = true)
        {
            _dbContext.Set<T>().Remove(entity);
            if( savechenges)
                await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbContext
                 .Set<T>()
                 .ToListAsync();
        }

        public async Task AddRangeAsync(IEnumerable<T> entities, bool savechenges = true)
        {
//            _dbContext.AttachRange(entities);
            foreach (var entity in entities)
            {
                _dbContext.Entry(entity).State = EntityState.Added;
            }
            //_dbContext.AddRangeAsync(entities);
            if (savechenges)
                await _dbContext.SaveChangesAsync();
        }


        public async Task UpdateRangeAsync(IEnumerable<T> entities, bool savechenges = true)
        {

    //        _dbContext.AttachRange(entities);
            foreach (var entity in entities)
            {
                _dbContext.Entry(entity).State = EntityState.Modified;
            }
            //_dbContext.UpdateRange(entities);
            if (savechenges)
                await _dbContext.SaveChangesAsync();
        }



        public async Task RemoveRangeAsync(IEnumerable<T> entities, bool savechenges = true)
        {
//            _dbContext.AttachRange(entities);
            foreach (var entity in entities)
            {
                _dbContext.Set<T>().Remove(entity);
            }
            //_dbContext.RemoveRange(entities);
            if (savechenges)
                await _dbContext.SaveChangesAsync();
        }

    }
}