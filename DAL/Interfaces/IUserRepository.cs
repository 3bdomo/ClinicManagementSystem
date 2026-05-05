using ClinicSystem.DAL.Models;
using Common.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<ApplicationUser>> GetAllAsync();
    Task<ApplicationUser?> GetByIdAsync(string id);
    Task<ApplicationUser?> GetByEmailAsync(string email);
    Task<IEnumerable<ApplicationUser>> GetByRoleAsync(UserRole role);
    Task<ApplicationUser?> CreateAsync(ApplicationUser user);
    Task<ApplicationUser?> UpdateAsync(ApplicationUser user);
    Task DeleteAsync(ApplicationUser user);
    Task<IEnumerable<ApplicationUser>> GetActiveAsync();
}

