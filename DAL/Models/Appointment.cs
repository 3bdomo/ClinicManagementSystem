using Common.Interfaces;
using Common.Enums;

namespace ClinicSystem.DAL.Models;

public class Appointment : IAuditable
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public int? DoctorScheduleId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public int DurationMinutes { get; set; }
    public AppointmentType AppointmentType { get; set; }
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Waiting;
    public string? Notes { get; set; }

    // IAuditable
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    public Patient Patient { get; set; } = null!;
    public Doctor Doctor { get; set; } = null!;
    public DoctorSchedule? DoctorSchedule { get; set; }
    public MedicalRecord? MedicalRecord { get; set; }
    public Invoice? Invoice { get; set; }
}