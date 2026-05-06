using ClinicSystem.DAL.Models;
using DAL.Context;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Common.Enums;


namespace DAL.Repositories;

internal class UserRepository : GenericRepository<ApplicationUser>, IUserRepository
{
    public UserRepository(ClinicDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ApplicationUser>> GetAllAsync()
    {
        return await _dbSet.OrderBy(u => u.FullName).ToListAsync();
    }

    public async Task<ApplicationUser?> GetByIdAsync(string id)
    {
        return await _dbSet.Include(u => u.Doctor)
                           .Include(u => u.Patient)
                           .Include(u => u.Receptionist)
                           //.Include(u => u.Admin)
                           .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<ApplicationUser?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<IEnumerable<ApplicationUser>> GetByRoleAsync(UserRole role)
    {
        return await _dbSet.Where(u => u.UserRole == role)
                           .OrderBy(u => u.FullName)
                           .ToListAsync();
    }

    public Task DeleteAsync(ApplicationUser user)
    {
        _dbSet.Remove(user);
        return _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<ApplicationUser>> GetActiveAsync()
    {
        return await _dbSet.Where(u => u.IsActive)
                           .OrderBy(u => u.FullName)
                           .ToListAsync();
    }
    
    
    public async Task<ApplicationUser?> CreateAsync(ApplicationUser user)
    {
        if (user.Email != null)
        {
            var existingUser = await GetByEmailAsync(user.Email);
            if (existingUser != null)
            {
                return null; // Email already exists
            }
        }
        user.CreatedAt = DateTime.UtcNow;
        var result = await _dbSet.AddAsync(user);
        await _context.SaveChangesAsync();
        return result.Entity;
    }

    public async Task<ApplicationUser?> UpdateAsync(ApplicationUser user)
    {
        var existingUser = await _dbSet.FindAsync(user.Id);
        if(existingUser == null)
        {
            return null;
        }
        existingUser.FullName = user.FullName;
        existingUser.PhoneNumber = user.PhoneNumber;
        existingUser.IsActive = user.IsActive;
        _dbSet.Update(existingUser);
        await _context.SaveChangesAsync();
        return existingUser;
    }

    public async Task<string?> GetFullNameAsync(string id)
    {
        return await _dbSet.Where(u => u.Id == id)
                           .Select(u => u.FullName)
                           .FirstOrDefaultAsync();
    }
}
