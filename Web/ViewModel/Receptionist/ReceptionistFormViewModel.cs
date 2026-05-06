using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels.Receptionist;

public class ReceptionistFormViewModel
{
    public int Id { get; set; }

    // ── Full Name ──────────────────────────────────────────────────
    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(100, MinimumLength = 3,
        ErrorMessage = "Full name must be between 3 and 100 characters.")]
    [RegularExpression(@"^[\p{L}\s\-'\.]+$",
        ErrorMessage = "Full name can only contain letters, spaces, hyphens, apostrophes, and dots.")]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    // ── Email ──────────────────────────────────────────────────────
    // Read-only display — NOT posted back to server
    // Email changes go through User Management
    [Display(Name = "Email Address")]
    public string? Email { get; set; }

    // ── Phone ──────────────────────────────────────────────────────
    [RegularExpression(@"^[\d\s\-\+\(\)]{7,20}$",
        ErrorMessage = "Phone number must be 7–20 characters and contain only digits, spaces, +, -, or parentheses.")]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    // ── Status ─────────────────────────────────────────────────────
    [Display(Name = "Active Account")]
    public bool IsActive { get; set; }
}