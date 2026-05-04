using System.ComponentModel.DataAnnotations;
using Common.Enums;

namespace BLL.DTOs.Patient;

public class PatientRegisterDto
{
    public string FullName { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    [Length(14,14, ErrorMessage = "National ID must be exactly 14 characters.")]
    [RegularExpression(@"^\d{14}$", ErrorMessage = "National ID must consist of exactly 14 digits.")]
    public string NationalId { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? BloodType { get; set; }
    public string? EmergencyContact { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
