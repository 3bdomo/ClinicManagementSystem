using DAL.Interfaces;
using AutoMapper;
using BLL.DTOs.User;
using BLL.Interfaces;
using ClinicSystem.DAL.Models;
using Common.Enums;
using Common.Results;
using Microsoft.AspNetCore.Identity;

namespace BLL.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userManager = userManager;
    }

    public Task<OperationResult<IEnumerable<UserDto>>> GetAllAsync()
    {
        var users = _unitOfWork.Users.GetAllAsync();
        return Task.FromResult(OperationResult<IEnumerable<UserDto>>.Success(
            users.Result.Select(u => _mapper.Map<UserDto>(u))));
    }

    public Task<OperationResult<UserDto>> GetByIdAsync(string id)
    {
        var user = _unitOfWork.Users.GetByIdAsync(id);
        if (user.Result != null)
            return Task.FromResult(OperationResult<UserDto>.Success(_mapper.Map<UserDto>(user.Result)));

        return Task.FromResult(OperationResult<UserDto>.Failure("User not found"));
    }

    public Task<OperationResult<IEnumerable<UserDto>>> GetByRoleAsync(UserRole role)
    {
        var users = _unitOfWork.Users.GetByRoleAsync(role);
        return Task.FromResult(OperationResult<IEnumerable<UserDto>>.Success(
            users.Result.Select(u => _mapper.Map<UserDto>(u))));
    }

    public async Task<OperationResult<string>> CreateAsync(CreateUserDto dto)
    {

        var appUser = _mapper.Map<ApplicationUser>(dto);
        appUser.UserRole = dto.UserRole;

        
        var identityResult = await _userManager.CreateAsync(appUser, dto.Password);
        if (!identityResult.Succeeded)
        {
            var errors = string.Join("; ", identityResult.Errors.Select(e => e.Description));
            return OperationResult<string>.Failure(errors);
        }

        
        var roleName = dto.UserRole.ToString();
        await _userManager.AddToRoleAsync(appUser, roleName);

        
        if (dto.UserRole == UserRole.Doctor)
        {
            var doctor = _mapper.Map<Doctor>(dto);
            doctor.ApplicationUserId = appUser.Id;
            doctor.FullName = appUser.FullName;
            doctor.Phone = appUser.PhoneNumber;
            await _unitOfWork.Doctors.AddAsync(doctor);
            await _unitOfWork.SaveChangesAsync();
        }
        else if (dto.UserRole == UserRole.Receptionist)
        {
            var receptionist = _mapper.Map<Receptionist>(dto);
            receptionist.ApplicationUserId = appUser.Id;
            await _unitOfWork.Receptionists.AddAsync(receptionist);
            await _unitOfWork.SaveChangesAsync();
        }

        return OperationResult<string>.Success("User created successfully");
    }

    public Task<OperationResult> UpdateAsync(UpdateUserDto dto)
    {
        var user = _unitOfWork.Users.GetByIdAsync(dto.Id).Result;
        if (user == null)
            return Task.FromResult(OperationResult.Failure("User not found"));

        user.FullName = dto.FullName;
        user.PhoneNumber = dto.PhoneNumber;
        user.IsActive = dto.IsActive;
        _unitOfWork.Users.UpdateAsync(user);
        _ = _unitOfWork.SaveChangesAsync();
        return Task.FromResult(OperationResult.Success("User updated successfully"));
    }

    public Task<OperationResult> ToggleActiveAsync(string id)
    {
        var user = _unitOfWork.Users.GetByIdAsync(id).Result;
        if (user == null)
            return Task.FromResult(OperationResult.Failure("User not found"));

        user.IsActive = !user.IsActive;
        _unitOfWork.Users.UpdateAsync(user);
        _ = _unitOfWork.SaveChangesAsync();
        return Task.FromResult(OperationResult.Success("User status toggled successfully"));
    }

    public async Task<OperationResult> ResetPasswordAsync(string id, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return OperationResult.Failure("User not found");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return OperationResult.Failure(errors);
        }

        return OperationResult.Success("Password reset successfully");
    }

    public Task<OperationResult> DeleteAsync(string id)
    {
        var user = _unitOfWork.Users.GetByIdAsync(id).Result;
        if (user == null)
            return Task.FromResult(OperationResult.Failure("User not found"));

        _unitOfWork.Users.DeleteAsync(user);
        _ = _unitOfWork.SaveChangesAsync();
        return Task.FromResult(OperationResult.Success("User deleted successfully"));
    }
}
