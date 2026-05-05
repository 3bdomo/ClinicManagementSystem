using Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModel;
public class DoctorFormViewModel
{
    public int Id { get; set; }
    [Required] public string FullName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    [Required] public Specialization Specialization { get; set; }
    [Required] public decimal ConsultationFee { get; set; }
    public string? Bio { get; set; }
    public bool IsAvailable { get; set; }
}
