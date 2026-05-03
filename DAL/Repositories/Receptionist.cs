using ClinicSystem.DAL.Models;
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
    internal class ReceptionistRepository : GenericRepository<Receptionist>, IReceptionistRepository
    {
        public ReceptionistRepository(ClinicDbContext context) : base(context)
        {
        }

        public async Task<Receptionist?> GetByUserIdAsync(string userId)
        {
            return await _dbSet
                .Include(r => r.ApplicationUser)
                .FirstOrDefaultAsync(r => r.ApplicationUserId == userId);
        }

        public async Task<Receptionist?> GetWithUserAsync(int receptionistId)
        {
            return await _dbSet
                .Include(r => r.ApplicationUser)
                .FirstOrDefaultAsync(r => r.Id == receptionistId);
        }

        public async Task<IEnumerable<Receptionist>> GetActiveAsync()
        {
            return await _dbSet
                .Include(r => r.ApplicationUser)
                .Where(r => r.IsActive)
                .OrderBy(r => r.FullName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Receptionist>> GetAllWithUsersAsync()
        {
            return await _dbSet
                .Include(r => r.ApplicationUser)
                .OrderBy(r => r.FullName)
                .ToListAsync();
        }

    }
}
