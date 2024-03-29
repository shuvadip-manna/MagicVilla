﻿using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Model;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MagicVilla_VillaAPI.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _dbContext;
        internal DbSet<T> _dbset;
        public Repository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbset = _dbContext.Set<T>();
        }
        public async Task CreateAsync(T entity)
        {
            await _dbset.AddAsync(entity);
            await SaveAsync();
        }
        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null)
        {
            IQueryable<T> query = _dbset;

            if (filter != null)
                query = query.Where(filter);

            return await query.ToListAsync();
        }
        public async Task<T> GetAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true)
        {
            IQueryable<T> query = _dbset;

            if (!tracked)
                query = query.AsNoTracking();
            if (filter != null)
                query = query.Where(filter);

            return await query.FirstOrDefaultAsync();
        }
        public async Task RemoveAsync(T entity)
        {
            _dbset.Remove(entity);
            await SaveAsync();
        }
        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
