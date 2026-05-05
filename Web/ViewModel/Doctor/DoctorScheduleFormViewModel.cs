using Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModel;
public class DoctorScheduleFormViewModel
{
    public int Id { get; set; }
    [Required] public int DoctorId { get; set; }
    public string? DoctorName { get; set; }
    [Required] public ScheduleType ScheduleType { get; set; }
    public DayOfWeek? DayOfWeek { get; set; }
    public DateOnly? SpecificDate { get; set; }
    [Required] public TimeOnly StartTime { get; set; }
    [Required] public TimeOnly EndTime { get; set; }
    [Range(5, 240)] public int SlotMinutes { get; set; } = 30;
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }
}
