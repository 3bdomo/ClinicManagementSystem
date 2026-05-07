using Common.Enums;
using Common.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ClinicSystem.DAL.Models;

public class ApplicationUser : IdentityUser, IAuditable
{
    public string FullName { get; set; } = string.Empty;
    public UserRole UserRole { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    
    public Doctor? Doctor { get; set; }
    public Patient? Patient { get; set; }
    public Receptionist? Receptionist { get; set; } 
}