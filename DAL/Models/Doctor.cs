using Common.Enums;
using Common.Interfaces;
namespace ClinicSystem.DAL.Models;

public class Doctor
{
    public int Id { get; set; }
    public string ApplicationUserId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public Specialization Specialization { get; set; }
    public string? Bio { get; set; }
    public decimal ConsultationFee { get; set; }
    public bool IsAvailable { get; set; } = true;

    // Navigation Properties
    public ApplicationUser ApplicationUser { get; set; } = null!;
    public ICollection<DoctorSchedule> DoctorSchedules { get; set; } = new List<DoctorSchedule>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
}