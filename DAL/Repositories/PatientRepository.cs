using ClinicSystem.DAL.Models;
using DAL.Context;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

internal class PatientRepository : GenericRepository<Patient>, IPatientRepository
{
    public PatientRepository(ClinicDbContext context) : base(context) { }

    public async Task<Patient?> GetByNationalIdAsync(string nationalId)
        => await _dbSet.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.NationalId == nationalId);

    public async Task<Patient?> GetByUserIdAsync(string applicationUserId)
        => await _dbSet
            .FirstOrDefaultAsync(p => p.ApplicationUserId == applicationUserId);

    public async Task<IEnumerable<Patient>> GetDeletedAsync()
        => await _context.Patients
            .IgnoreQueryFilters()
            .Where(p => p.IsDeleted)
            .AsNoTracking()
            .ToListAsync();

    public async Task<Patient?> GetWithFullHistoryAsync(int patientId)
        => await _dbSet
            .Include(p => p.Appointments)
            .Include(p => p.MedicalRecords)
            .Include(p => p.Invoices)
            .FirstOrDefaultAsync(p => p.Id == patientId);

    public async Task<bool> RestoreAsync(int patientId)
    {
        var patient = await _dbSet
            .Include(p => p.ApplicationUser)
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == patientId && p.IsDeleted);

        if (patient is null) return false;

        patient.IsDeleted  = false;
        patient.DeletedAt  = null;
        patient.DeletedBy  = null;
        
        if (patient.ApplicationUser != null && patient.ApplicationUser.IsDeleted)
        {
            patient.ApplicationUser.IsDeleted = false;
            patient.ApplicationUser.DeletedAt = null;
            patient.ApplicationUser.DeletedBy = null;
        }

        return true;
    }

    public async Task<IEnumerable<Patient>> SearchAsync(string query)
        => await _dbSet
            .Where(p => p.FullName.Contains(query) || p.NationalId.Contains(query))
            .ToListAsync();
}
