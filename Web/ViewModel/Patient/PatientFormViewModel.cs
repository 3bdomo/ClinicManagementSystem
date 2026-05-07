using Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModel;
public class PatientFormViewModel
{
    public int Id { get; set; }

    [Required] 
    public string FullName { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    [Required] 
    public DateOnly DateOfBirth { get; set; }

    [Required]
    public Gender Gender { get; set; }

    [Required(ErrorMessage = "National ID is required")]
    [StringLength(14, MinimumLength = 14, ErrorMessage = "National ID must be exactly 14 digits")]
    [RegularExpression(@"^\d{14}$", ErrorMessage = "National ID must be numeric")]
    public string NationalId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "Phone number must be exactly 11 digits")]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "Phone number must be numeric")]
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? BloodType { get; set; }
    public string? EmergencyContact { get; set; }
}
