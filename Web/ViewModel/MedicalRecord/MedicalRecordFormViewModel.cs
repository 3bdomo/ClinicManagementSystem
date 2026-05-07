using Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;

namespace Web.ViewModel;

public class MedicalRecordFormViewModel
{
    public int Id { get; set; }
    [Required] public int AppointmentId { get; set; }
    [Required] public string Diagnosis { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime? FollowUpDate { get; set; }
    public byte[]? RowVersion { get; set; }

    public int? DoctorId { get; set; }
    public DateTime? VisitedDate { get; set; }

    public IEnumerable<IFormFile> NewAttachments { get; set; } = Enumerable.Empty<IFormFile>();
    public int? PatientId { get; set; }
    public IEnumerable<SelectListItem> Appointments { get; set; } = Enumerable.Empty<SelectListItem>();
}
