using Common.Enums;
using Common.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ClinicSystem.DAL.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public UserRole UserRole { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }

    // Navigation Properties
    public Doctor? Doctor { get; set; }
    public Patient? Patient { get; set; }
    public Receptionist? Receptionist { get; set; } 
}