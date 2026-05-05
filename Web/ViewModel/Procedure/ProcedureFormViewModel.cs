using Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModel;
public class ProcedureFormViewModel
{
    public int Id { get; set; }
    [Required] public int MedicalRecordId { get; set; }
    [Required] public int ProcedureTypeId { get; set; }
    public DateTime PerformedAt { get; set; } = DateTime.Now;
    public int? DurationMinutes { get; set; }
    public string? Notes { get; set; }
    public string? AfterCareNotes { get; set; }
    [Range(0, 1000000)] public decimal Cost { get; set; }
    public IEnumerable<SelectListItem> AvailableTypes { get; set; } = [];
    public IEnumerable<SelectListItem> MedicalRecords { get; set; } = [];
}
