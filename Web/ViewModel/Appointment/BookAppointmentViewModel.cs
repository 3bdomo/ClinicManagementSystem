using Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModel;
public class BookAppointmentViewModel
{
    [Required] public int PatientId { get; set; }
    [Required] public int DoctorId { get; set; }
    [Required] public DateTime AppointmentDate { get; set; }
    [Required] public AppointmentType AppointmentType { get; set; }
    public string? Notes { get; set; }
    public IEnumerable<SelectListItem> Patients { get; set; } = [];
    public IEnumerable<SelectListItem> Doctors { get; set; } = [];
}
