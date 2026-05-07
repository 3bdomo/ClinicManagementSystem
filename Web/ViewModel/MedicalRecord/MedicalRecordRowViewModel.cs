using Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModel;
public class MedicalRecordRowViewModel
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public int AppointmentId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;
    public DateTime VisitDate { get; set; }
    public string Diagnosis { get; set; } = string.Empty;
    public DateTime? FollowUpDate { get; set; }
    public string? Notes { get; set; }
    public int ProceduresCount { get; set; }
    public int AttachmentsCount { get; set; }
}
