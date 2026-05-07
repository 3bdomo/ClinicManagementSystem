using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels.Receptionist;

public class ReceptionistFormViewModel
{
    public int Id { get; set; }

    
    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(100, MinimumLength = 3,
        ErrorMessage = "Full name must be between 3 and 100 characters.")]
    [RegularExpression(@"^[\p{L}\s\-'\.]+$",
        ErrorMessage = "Full name can only contain letters, spaces, hyphens, apostrophes, and dots.")]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    
    
    
    [Display(Name = "Email Address")]
    public string? Email { get; set; }

    
    [RegularExpression(@"^[\d\s\-\+\(\)]{7,20}$",
        ErrorMessage = "Phone number must be 7–20 characters and contain only digits, spaces, +, -, or parentheses.")]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    
    [Display(Name = "Active Account")]
    public bool IsActive { get; set; }
}