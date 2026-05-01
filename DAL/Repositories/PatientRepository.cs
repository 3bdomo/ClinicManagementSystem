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

    internal class PatientRepository : GenericRepository<Patient>, IPatientRepository
    {
        public PatientRepository(ClinicDbContext context) : base(context)
        {
        }

        public async Task<Patient?> GetByNationalIdAsync(string nationalId)
            => await _dbSet
                .FirstOrDefaultAsync(p => p.NationalId == nationalId && !p.IsDeleted);

        //review: should we use string or int for userId? if it's string, we need to change the type of ApplicationUserId in Patient model
        public async Task<Patient?> GetByUserIdAsync(int userId)
            => await _dbSet
                .FirstOrDefaultAsync(p => p.Id== userId && !p.IsDeleted);

        public async Task<IEnumerable<Patient>> GetDeletedAsync()
            => await _dbSet
                .Where(p => p.IsDeleted)
                .ToListAsync();

        public async Task<Patient?> GetWithFullHistoryAsync(int patientId)
            => await _dbSet
                .Include(p => p.Appointments)
                .Include(p => p.Invoices)
                .FirstOrDefaultAsync(p => p.Id == patientId && !p.IsDeleted);

        public async Task<bool> RestoreAsync(int patientId)
        {
            var patient = await _dbSet
                .FirstOrDefaultAsync(p => p.Id == patientId && p.IsDeleted);

            if (patient == null)
                return false;

            patient.IsDeleted = false;
            _dbSet.Update(patient);

            return true;
        }

        public async Task<IEnumerable<Patient>> SearchAsync(string query)
            => await _dbSet
                .Where(p => p.FullName.Contains(query) && !p.IsDeleted)
                .ToListAsync();
    }
}
