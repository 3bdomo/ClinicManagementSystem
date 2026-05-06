using Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModel;
public class CreateUserViewModel
{
    [Required] public string FullName { get; set; } = string.Empty;
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    [Required, DataType(DataType.Password)] public string Password { get; set; } = string.Empty;
    [Compare(nameof(Password)), DataType(DataType.Password)] public string ConfirmPassword { get; set; } = string.Empty;
    [Required] public UserRole UserRole { get; set; }
    public Specialization? Specialization { get; set; }
    [Display(Name = "Consultation Fee (EGP)")] public decimal? ConsultationFee { get; set; }
    public string? Bio { get; set; }

    // Patient Fields
    [Display(Name = "National ID")] public string? NationalId { get; set; }
    [Display(Name = "Date of Birth")] [DataType(DataType.Date)] public DateOnly? DateOfBirth { get; set; }
    public Gender? Gender { get; set; }
    public string? Address { get; set; }
    [Display(Name = "Blood Type")] public string? BloodType { get; set; }
    [Display(Name = "Emergency Contact")] public string? EmergencyContact { get; set; }
}
