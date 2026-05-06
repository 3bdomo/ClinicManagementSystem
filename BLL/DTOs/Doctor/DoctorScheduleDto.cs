using Common.Enums;

namespace BLL.DTOs.Doctor;

public class DoctorScheduleDto
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    public string? DoctorName { get; set; }
    public ScheduleType ScheduleType { get; set; }
    public DayOfWeek? DayOfWeek { get; set; }
    public DateOnly? SpecificDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int SlotMinutes { get; set; }
    public bool IsActive { get; set; }
    public string? Notes { get; set; }
}
