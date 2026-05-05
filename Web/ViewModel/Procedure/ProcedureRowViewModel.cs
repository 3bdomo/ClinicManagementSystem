using Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModel;
public class ProcedureRowViewModel
{
    public int Id { get; set; }
    public int MedicalRecordId { get; set; }
    public string ProcedureTypeName { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public int PatientId { get; set; }
    public DateTime PerformedAt { get; set; }
    public int? DurationMinutes { get; set; }
    public decimal Cost { get; set; }
    public string? Notes { get; set; }
    public string? AfterCareNotes { get; set; }
}
