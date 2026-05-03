using ClinicSystem.DAL.Models;

namespace DAL.Interfaces;

public interface IDoctorRepository : IGenericRepository<Doctor>
{
    Task<IEnumerable<Doctor>> GetAvailableAsync();
    Task<Doctor?> GetWithSchedulesAsync(int doctorId);
    Task<Doctor?> GetByUserIdAsync(string applicationUserId);
    Task<Doctor?> GetByEmailAsync(string email);
}
