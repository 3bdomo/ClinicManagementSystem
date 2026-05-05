using Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModel;
public class DoctorDetailsViewModel
{
    public DoctorFormViewModel Doctor { get; set; } = new();
    public IEnumerable<DoctorScheduleFormViewModel> Schedules { get; set; } = [];
    public IEnumerable<AppointmentRowViewModel> TodayAppointments { get; set; } = [];
    public int TotalAppointmentsCount { get; set; }
}
