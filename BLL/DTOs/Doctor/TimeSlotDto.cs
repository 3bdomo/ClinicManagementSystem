using Common.Enums;

namespace BLL.DTOs.Doctor;

public class TimeSlotDto
{
    public int DoctorId { get; set; }
    public DateTime SlotStart { get; set; }
    public DateTime SlotEnd { get; set; }
    public bool IsAvailable { get; set; }
    public ScheduleType ScheduleType { get; set; }
    public int SlotMinutes { get; set; }
}