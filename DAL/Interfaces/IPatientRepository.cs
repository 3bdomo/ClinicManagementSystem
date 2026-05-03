using ClinicSystem.DAL.Models;

namespace DAL.Interfaces;

public interface IPatientRepository : IGenericRepository<Patient>
{
    Task<IEnumerable<Patient>> SearchAsync(string query);
    Task<Patient?> GetWithFullHistoryAsync(int patientId);
    Task<IEnumerable<Patient>> GetDeletedAsync();
    Task<bool> RestoreAsync(int patientId);
    Task<Patient?> GetByNationalIdAsync(string nationalId);
    Task<Patient?> GetByUserIdAsync(string applicationUserId);
}
