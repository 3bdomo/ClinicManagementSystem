using Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModel;
public class AppointmentListViewModel
{
    public IEnumerable<AppointmentRowViewModel> Appointments { get; set; } = [];
    public int? DoctorId { get; set; }
    public DateTime? SelectedDate { get; set; }
    public AppointmentType? AppointmentType { get; set; }
    public IEnumerable<SelectListItem> Doctors { get; set; } = [];
}
