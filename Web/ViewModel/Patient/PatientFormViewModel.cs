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
    [Required, StringLength(14, MinimumLength = 14)] public string NationalId { get; set; } = string.Empty;
    [Required] public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
    public BloodType? BloodType { get; set; }
    public string? EmergencyContact { get; set; }
}
