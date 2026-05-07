using Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModel;
public class PatientFormViewModel
{
    public int Id { get; set; }
    [Required] public string FullName { get; set; } = string.Empty;
    [Required] public DateOnly DateOfBirth { get; set; }
    [Required] public Gender Gender { get; set; }
    [RegularExpression(@"^\d{14}$", ErrorMessage = "National ID must be exactly 14 digits.")]
    [Required, StringLength(14, MinimumLength = 14)] public string NationalId { get; set; } = string.Empty;

    [StringLength(11, MinimumLength = 11)]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "Phone number must be exactly 11 digits.")]
    [Required] public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? BloodType { get; set; }
    public string? EmergencyContact { get; set; }
}
