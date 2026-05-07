using Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Web.ViewModel;
public class DoctorScheduleFormViewModel : IValidatableObject
{
    public int Id { get; set; }
    [Required] public int DoctorId { get; set; }
    public string? DoctorName { get; set; }
    [Required] public ScheduleType ScheduleType { get; set; }
    public DayOfWeek? DayOfWeek { get; set; }
    public DateOnly? SpecificDate { get; set; }
    [Required] public TimeOnly StartTime { get; set; }
    [Required]
    public TimeOnly EndTime { get; set; }
    [Range(5, 240)] public int SlotMinutes { get; set; } = 30;
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }
    public IEnumerable<SelectListItem> Doctors { get; set; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartTime >= EndTime)
        {
            yield return new ValidationResult(
                "Start time must be before end time.",
                [nameof(StartTime), nameof(EndTime)]);
        }

     

        if (SpecificDate.HasValue && SpecificDate.Value < DateOnly.FromDateTime(DateTime.Today))
        {
            yield return new ValidationResult(
                "Specific date must be in the future.",
                [nameof(SpecificDate)]);
        }
    }
}
