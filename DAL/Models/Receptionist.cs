using Common.Interfaces;

namespace ClinicSystem.DAL.Models;

public class Receptionist : IAuditable
{
    public int Id { get; set; }
    public string ApplicationUserId { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    // IAuditable
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    public ApplicationUser ApplicationUser { get; set; } = null!;
}