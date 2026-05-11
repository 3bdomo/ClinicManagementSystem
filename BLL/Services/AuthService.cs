using AutoMapper;
using BLL.DTOs.Auth;
using BLL.Interfaces;
using ClinicSystem.DAL.Models;
using Common.Enums;
using Common.Results;
using DAL.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace BLL.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AuthService(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<OperationResult<string>> RegisterAsync(RegisterDto model)
    {
        var user = _mapper.Map<ApplicationUser>(model);
        user.UserRole = UserRole.Patient;

        using var transaction = await _unitOfWork.BeginTransactionAsync();
        
        try
        {
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
               // var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return OperationResult<string>.Failure("Failed to register user.");
            }

            var patient = _mapper.Map<Patient>(model);
            patient.ApplicationUserId = user.Id;
            await _unitOfWork.Patients.AddAsync(patient);
            
            await _unitOfWork.SaveChangesAsync();
            await transaction.CommitAsync();
            
            return OperationResult<string>.Success("User registered successfully.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return OperationResult<string>.Failure("Failed to create role-specific profile: " + ex.Message);
        }
    }

    public async Task<OperationResult<ClaimsPrincipal>> LoginAsync(LoginDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
        {
            return OperationResult<ClaimsPrincipal>.Failure("Invalid email or password.");
        }

        if (!user.IsActive)
        {
            return OperationResult<ClaimsPrincipal>.Failure("Account is deactivated.");
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id), 
            new Claim(ClaimTypes.Name, user.FullName ?? user.UserName ?? ""),
            new Claim(ClaimTypes.Role, user.UserRole.ToString())
        };

        var identity = new ClaimsIdentity(claims, "Cookies");
        var principal = new ClaimsPrincipal(identity);

        return OperationResult<ClaimsPrincipal>.Success(principal);
    }

    public async Task<OperationResult<string>> RegisterAsync(RegisterDto model, UserRole role = UserRole.Patient)
    {
        var user = _mapper.Map<ApplicationUser>(model);
        user.UserRole = role;

        using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return OperationResult<string>.Failure(errors);
            }

            var patient = _mapper.Map<Patient>(model);
            patient.ApplicationUserId = user.Id;
            await _unitOfWork.Patients.AddAsync(patient);

            await _unitOfWork.SaveChangesAsync();
            await transaction.CommitAsync();

            return OperationResult<string>.Success("User registered successfully.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return OperationResult<string>.Failure("Failed to create role-specific profile: " + ex.Message);
        }
    }
}
