using Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModel;
public class InvoiceRowViewModel
{
    public int Id { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public int PatientId { get; set; }
    public int AppointmentId { get; set; }
    public string? DoctorName { get; set; }
    public decimal TotalAmount { get; set; }
    public InvoiceStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PaidAt { get; set; }
}
