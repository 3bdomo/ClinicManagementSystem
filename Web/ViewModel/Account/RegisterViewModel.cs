using Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModel;
public class RegisterViewModel
{
    [Required] public string FullName { get; set; } = string.Empty;
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
    [Required, DataType(DataType.Password), MinLength(8)] public string Password { get; set; } = string.Empty;
    [Compare(nameof(Password)), DataType(DataType.Password)] public string ConfirmPassword { get; set; } = string.Empty;
    [Required, StringLength(14, MinimumLength = 14)] public string NationalId { get; set; } = string.Empty;
    [Required] public string Phone { get; set; } = string.Empty;
    [Required, DataType(DataType.Date)] public DateOnly DateOfBirth { get; set; }
    [Required] public Gender Gender { get; set; }
    public string? Address { get; set; }
    public string? BloodType { get; set; }
    public string? EmergencyContact { get; set; }
}
