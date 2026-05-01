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
    internal class ProcedureRepository : GenericRepository<Procedure>, IProcedureRepository
    {
        public ProcedureRepository(ClinicDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Procedure>> GetByMedicalRecordAsync(int recordId)
     => await _dbSet
         .Where(p => p.MedicalRecordId == recordId)
         .ToListAsync();

        public async Task<IEnumerable<Procedure>> GetByPatientAsync(int patientId)
            => await _dbSet
                .Where(p => p.MedicalRecord.PatientId == patientId)
                .ToListAsync();
    }
}