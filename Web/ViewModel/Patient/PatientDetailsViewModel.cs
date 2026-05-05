using Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModel;
public class PatientDetailsViewModel
{
    public PatientRowViewModel Patient { get; set; } = new();
    public IEnumerable<AppointmentRowViewModel> Appointments { get; set; } = [];
    public IEnumerable<MedicalRecordRowViewModel> MedicalRecords { get; set; } = [];
    public IEnumerable<InvoiceRowViewModel> Invoices { get; set; } = [];
    public AuditInfoViewModel AuditInfo { get; set; } = new();
}
