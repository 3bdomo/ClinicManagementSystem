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
    [Required, StringLength(14, MinimumLength = 14), RegularExpression(@"^\d{14}$", ErrorMessage = "National ID must be exactly 14 digits.")] public string NationalId { get; set; } = string.Empty;
    [Required, Phone, RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "Invalid Egyptian phone number")] public string Phone { get; set; } = string.Empty;
    [Required, DataType(DataType.Date)] public DateOnly DateOfBirth { get; set; }
    [Required] public Gender Gender { get; set; }
    public string? Address { get; set; }
    public string? BloodType { get; set; }
    public string? EmergencyContact { get; set; }
}
