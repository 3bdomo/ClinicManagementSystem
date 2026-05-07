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

    public async Task<OperationResult<IEnumerable<UserDto>>> GetAllAsync()
    {
        var users = await _unitOfWork.Users.GetAllAsync();
        return OperationResult<IEnumerable<UserDto>>.Success(
            users.Select(u => _mapper.Map<UserDto>(u)));
    }

    public async Task<OperationResult<UserDto>> GetByIdAsync(string id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user != null)
            return OperationResult<UserDto>.Success(_mapper.Map<UserDto>(user));

        return OperationResult<UserDto>.Failure("User not found");
    }

    public async Task<OperationResult<IEnumerable<UserDto>>> GetByRoleAsync(UserRole role)
    {
        var users = await _unitOfWork.Users.GetByRoleAsync(role);
        return OperationResult<IEnumerable<UserDto>>.Success(
            users.Select(u => _mapper.Map<UserDto>(u)));
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
        else if (dto.UserRole == UserRole.Patient)
        {
            var patient = new Patient
            {
                ApplicationUserId = appUser.Id,
                FullName = appUser.FullName,
                Phone = appUser.PhoneNumber ?? string.Empty,
                NationalId = dto.NationalId ?? string.Empty,
                DateOfBirth = dto.DateOfBirth ?? DateOnly.FromDateTime(DateTime.Today.AddYears(-20)),
                Gender = dto.Gender ?? Gender.Male,
                Address = dto.Address,
                BloodType = dto.BloodType,
                EmergencyContact = dto.EmergencyContact
            };
            await _unitOfWork.Patients.AddAsync(patient);
            await _unitOfWork.SaveChangesAsync();
        }

        return OperationResult<string>.Success("User created successfully");
    }

    public async Task<OperationResult> UpdateAsync(UpdateUserDto dto)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(dto.Id);
        if (user == null)
            return OperationResult.Failure("User not found");

        user.FullName = dto.FullName;
        user.PhoneNumber = dto.PhoneNumber;
        user.IsActive = dto.IsActive;
        
        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
        return OperationResult.Success("User updated successfully");
    }

    public async Task<OperationResult> ToggleActiveAsync(string id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null)
            return OperationResult.Failure("User not found");

        user.IsActive = !user.IsActive;
        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
        return OperationResult.Success("User status toggled successfully");
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

    public async Task<OperationResult> DeleteAsync(string id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null)
            return OperationResult.Failure("User not found");

        if (user.Patient != null)
        {
            user.Patient.IsDeleted = true;
            user.Patient.DeletedAt = DateTime.UtcNow;
            _unitOfWork.Patients.Update(user.Patient);
            await _unitOfWork.SaveChangesAsync();
        }
        
        if (user.Doctor != null)
        {
            _unitOfWork.Doctors.Delete(user.Doctor);
            await _unitOfWork.SaveChangesAsync();
        }

        if (user.Receptionist != null)
        {
            _unitOfWork.Receptionists.Delete(user.Receptionist);
            await _unitOfWork.SaveChangesAsync();
        }

        await _unitOfWork.Users.DeleteAsync(user);
        await _unitOfWork.SaveChangesAsync();
        return OperationResult.Success("User and linked profiles deleted successfully");
    }
}
