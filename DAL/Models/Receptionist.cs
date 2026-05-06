using Common.Interfaces;

namespace ClinicSystem.DAL.Models;

public class Receptionist : IAuditable
{
    public int Id { get; set; }
    public string ApplicationUserId { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public bool IsActive { get; set; } = true;

    
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }

    
    public ApplicationUser ApplicationUser { get; set; } = null!;
}