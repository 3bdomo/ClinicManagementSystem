using Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModel;
public class MedicalRecordDetailsViewModel
{
    public MedicalRecordRowViewModel Record { get; set; } = new();
    public IEnumerable<ProcedureRowViewModel> Procedures { get; set; } = [];
    public AuditInfoViewModel AuditInfo { get; set; } = new();
    public bool CanEdit { get; set; }
}
