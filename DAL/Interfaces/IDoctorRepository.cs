using ClinicSystem.DAL.Models;
using Common.Enums;

namespace DAL.Interfaces;

public interface IDoctorRepository : IGenericRepository<Doctor>
{
    Task<IEnumerable<Doctor>> GetBySpecializationAsync(Specialization specialization, int pageNumber, int pageSize);
    Task<IEnumerable<Doctor>> GetAvailableAsync();
    Task<Doctor?> GetWithSchedulesAsync(int doctorId);
    Task<Doctor?> GetByUserIdAsync(string applicationUserId);
    Task<Doctor?> GetByEmailAsync(string email);
}
