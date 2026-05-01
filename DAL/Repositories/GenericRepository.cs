using DAL.Context;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ClinicDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ClinicDbContext context, DbSet<T> dbSet)
        {
            _context = context;
            _dbSet = dbSet;
        }

        public async Task<IEnumerable<T>> GetAllAsync(int pageNumber, int pageSize)
        {
            return await _dbSet.Skip((pageNumber - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
            => await _dbSet.FindAsync(id);
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression)
            => await _dbSet.Where(expression).ToListAsync();
        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> expression)
            => await _dbSet.FirstOrDefaultAsync(expression);

        public async Task AddAsync(T entity)
           => await _dbSet.AddAsync(entity);

        public void Update(T entity)
            => _dbSet.Update(entity);

        public void Delete(T entity)
            => _dbSet.Remove(entity);

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();


    }
}
