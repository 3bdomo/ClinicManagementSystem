using Common.Interfaces;
using Common.Enums;

namespace ClinicSystem.DAL.Models;

public class DoctorSchedule : IAuditable
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    public ScheduleType ScheduleType { get; set; }
    public DayOfWeek? DayOfWeek { get; set; }
    public DateOnly? SpecificDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int SlotMinutes { get; set; } = 30;
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }

    // IAuditable
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    public Doctor Doctor { get; set; } = null!;
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}