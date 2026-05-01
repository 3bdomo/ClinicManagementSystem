using Common.Interfaces;
namespace ClinicSystem.DAL.Models;

public class MedicalRecord : IAuditable
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public int AppointmentId { get; set; }
    public string Diagnosis { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime VisitDate { get; set; }
    public DateTime? FollowUpDate { get; set; }

    // IAuditable
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    public Patient Patient { get; set; } = null!;
    public Doctor Doctor { get; set; } = null!;
    public Appointment Appointment { get; set; } = null!;
    public ICollection<RecordAttachment> Attachments { get; set; } = new List<RecordAttachment>();
    public ICollection<Procedure> Procedures { get; set; } = new List<Procedure>();
}