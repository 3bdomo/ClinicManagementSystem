using Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModel;
public class DoctorScheduleListViewModel
{
    public int? DoctorId { get; set; }
    public string? DoctorName { get; set; }
    public ScheduleType? ScheduleType { get; set; }
    public IEnumerable<DoctorScheduleFormViewModel> Schedules { get; set; } = [];
    public IEnumerable<SelectListItem> Doctors { get; set; } = [];
}
