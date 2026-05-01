using ClinicSystem.DAL.Models;
using Common.Enums;
using DAL.Context;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    internal class DoctorRepository : GenericRepository<Doctor>, IDoctorRepository
    {
        public DoctorRepository(ClinicDbContext context) : base(context) 
    {
    }

    public async Task<IEnumerable<Doctor>> GetAvailableDoctorsAsync()
        => await _dbSet.Where(d => d.IsAvailable).ToListAsync();

        public async Task<Doctor?> GetByEmailAsync(string email)
            => await _dbSet
                .Include(d => d.ApplicationUser)
                .FirstOrDefaultAsync(d => d.ApplicationUser.Email == email);

        public async Task<IEnumerable<Doctor>> GetBySpecializationAsync(Specialization spec)
            => await _dbSet.Where(d => d.Specialization == spec).ToListAsync();

        public async Task<Doctor?> GetWithUserAsync(int doctorId)
            => await _dbSet
                .Include(d => d.ApplicationUser)
                .FirstOrDefaultAsync(d => d.Id == doctorId);
    }
}