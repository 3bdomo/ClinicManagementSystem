using BLL.DTOs.Auth;
using Common.Enums;
using Common.Results;
using System.Security.Claims;

namespace BLL.Interfaces;

public interface IAuthService
{
    Task<OperationResult<string>> RegisterAsync(RegisterDto model, UserRole role = UserRole.Patient);
    Task<OperationResult<ClaimsPrincipal>> LoginAsync(LoginDto model);
}
