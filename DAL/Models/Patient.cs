using Common.Interfaces;
using Common.Enums;

namespace ClinicSystem.DAL.Models;

public class Patient : IAuditable, ISoftDeletable
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public string NationalId { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? BloodType { get; set; }
    public string? EmergencyContact { get; set; }
    public string? ApplicationUserId { get; set; }

    // IAuditable
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // ISoftDeletable
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Navigation Properties
    public ApplicationUser? ApplicationUser { get; set; }
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}