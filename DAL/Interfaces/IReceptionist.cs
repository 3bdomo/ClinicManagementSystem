using ClinicSystem.DAL.Models;

namespace DAL.Interfaces;

public interface IReceptionistRepository : IGenericRepository<Receptionist>
{
    Task<Receptionist?> GetByUserIdAsync(string userId);
    Task<Receptionist?> GetWithUserAsync(int receptionistId);
    Task<IEnumerable<Receptionist>> GetActiveAsync();
    Task<IEnumerable<Receptionist>> GetAllWithUsersAsync();
}
