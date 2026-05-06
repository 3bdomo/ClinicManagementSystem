using DAL.Interfaces;
using System.Security.Cryptography;
using AutoMapper;
using BLL.DTOs.User;
using BLL.Interfaces;
using Common.Enums;
using Common.Results;
using DAL.Repositories;
using Microsoft.AspNetCore.Identity;

namespace BLL.Services;

public class UserService:IUserService
{
    
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public UserService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public Task<OperationResult<IEnumerable<UserDto>>> GetAllAsync()
    {
       var users=_unitOfWork.Users.GetAllAsync();
       return Task.FromResult(OperationResult<IEnumerable<UserDto>>.Success(users.Result.Select(u=>_mapper.Map<UserDto>(u))));
    }

    public Task<OperationResult<UserDto>> GetByIdAsync(string id)
    {
        var user=_unitOfWork.Users.GetByIdAsync(id);
        if (user.Result != null)
        {
            return Task.FromResult(OperationResult<UserDto>.Success(_mapper.Map<UserDto>(user.Result)));
        }
        return Task.FromResult(OperationResult<UserDto>.Failure("User not found"));
    }

    public Task<OperationResult<IEnumerable<UserDto>>> GetByRoleAsync(UserRole role)
    {
        var users=_unitOfWork.Users.GetByRoleAsync(role);
        return Task.FromResult(OperationResult<IEnumerable<UserDto>>.Success(users.Result.Select(u=>_mapper.Map<UserDto>(u))));
    }

    public Task<OperationResult<string>> CreateAsync(CreateUserDto dto)
    {
        if (_unitOfWork.Users.GetByEmailAsync(dto.Email).Result != null)
        {
            return Task.FromResult(OperationResult<string>.Failure("Email already exists"));
            
        }
        _unitOfWork.Users.CreateAsync(_mapper.Map<ClinicSystem.DAL.Models.ApplicationUser>(dto));
        _ = _unitOfWork.SaveChangesAsync();
        return Task.FromResult(OperationResult<string>.Success("User created successfully"));
    }

    public Task<OperationResult> UpdateAsync(UpdateUserDto dto)
    {
        var user=_unitOfWork.Users.GetByIdAsync(dto.Id).Result;
        if (user == null)
        {
            return Task.FromResult(OperationResult.Failure("User not found"));
        }
        user.FullName = dto.FullName;
        user.PhoneNumber = dto.PhoneNumber;
        user.IsActive = dto.IsActive;
        _unitOfWork.Users.UpdateAsync(user);
        _ = _unitOfWork.SaveChangesAsync();
        return Task.FromResult(OperationResult.Success("User updated successfully"));
    }

    public Task<OperationResult> ToggleActiveAsync(string id)
    {
        var user=_unitOfWork.Users.GetByIdAsync(id).Result;
        if (user == null)
        {
            return Task.FromResult(OperationResult.Failure("User not found"));
        }
        user.IsActive = !user.IsActive;
        _unitOfWork.Users.UpdateAsync(user);
        _ = _unitOfWork.SaveChangesAsync();
        return Task.FromResult(OperationResult.Success("User status toggled successfully"));
    }
    

    public Task<OperationResult> ResetPasswordAsync(string id, string newPassword)
    {
        var user=_unitOfWork.Users.GetByIdAsync(id).Result;
        if (user == null)
        {
            return Task.FromResult(OperationResult.Failure("User not found"));
        }
        var hasher = new PasswordHasher<object>();
        user.PasswordHash =hasher.HashPassword(user ,newPassword); 
        _unitOfWork.Users.UpdateAsync(user);
        _ = _unitOfWork.SaveChangesAsync();
        return Task.FromResult(OperationResult.Success("Password reset successfully"));
    }

    public Task<OperationResult> DeleteAsync(string id)
    {
        var user=_unitOfWork.Users.GetByIdAsync(id).Result;
        if (user == null)
        {
            return Task.FromResult(OperationResult.Failure("User not found"));
        }
        _unitOfWork.Users.DeleteAsync(user);
        _ = _unitOfWork.SaveChangesAsync();
        return Task.FromResult(OperationResult.Success("User deleted successfully"));
    }
}
