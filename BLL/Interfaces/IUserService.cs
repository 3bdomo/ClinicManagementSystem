using BLL.DTOs.User;
using Common.Enums;
using Common.Results;

namespace BLL.Interfaces;

public interface IUserService
{
    Task<OperationResult<IEnumerable<UserDto>>> GetAllAsync();
    Task<OperationResult<UserDto>> GetByIdAsync(string id);
    Task<OperationResult<IEnumerable<UserDto>>> GetByRoleAsync(UserRole role);
    Task<OperationResult<string>> CreateAsync(CreateUserDto dto);
    Task<OperationResult> UpdateAsync(UpdateUserDto dto);
    Task<OperationResult> ToggleActiveAsync(string id);
    Task<OperationResult> ResetPasswordAsync(string id, string newPassword);
    Task<OperationResult> DeleteAsync(string id);
}