using ClinicSystem.DAL.Models;
using DAL.Context;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    internal class ProcedureTypeRepository : GenericRepository<ProcedureType>, IProcedureTypeRepository
    {
        public ProcedureTypeRepository(ClinicDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ProcedureType>> GetActiveAsync()
        => await _dbSet
        .Where(pt => pt.IsActive)
        .ToListAsync();
    }
}