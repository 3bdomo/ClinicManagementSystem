using Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModel;
public class ProcedureDetailsViewModel
{
    public ProcedureRowViewModel Procedure { get; set; } = new();
    public MedicalRecordRowViewModel MedicalRecord { get; set; } = new();
    public AuditInfoViewModel AuditInfo { get; set; } = new();
}
