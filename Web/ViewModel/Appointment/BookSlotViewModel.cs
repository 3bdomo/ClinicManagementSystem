using Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModel;
public class BookSlotViewModel
{
    public Specialization? Specialization { get; set; }
    [Required] public int DoctorId { get; set; }
    [Required] public DateTime SelectedDate { get; set; }
    public AppointmentType AppointmentType { get; set; } = AppointmentType.Consultation;
    [Required] public DateTime SelectedSlot { get; set; }
    public IEnumerable<SelectListItem> Doctors { get; set; } = [];
}
