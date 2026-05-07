using Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModel;
public class InvoiceFormViewModel
{
    [Required] public int AppointmentId { get; set; }
    public int PatientId { get; set; }
    public string? PatientName { get; set; }
    public string? DoctorName { get; set; }
    public List<InvoiceItemFormModel> Items { get; set; } = [];
    public IEnumerable<SelectListItem> Appointments { get; set; } = [];
    public IEnumerable<SelectListItem> ProcedureTypes { get; set; } = [];
}
