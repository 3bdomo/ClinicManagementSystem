using Common.Enums;

namespace BLL.DTOs.Patient;

public class DoctorDto
{
    
    public int Id { get; set; }
    public string? ApplicationUserId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public Specialization Specialization { get; set; }
    public string? Bio { get; set; }
    public decimal? ConsultationFee { get; set; }
    public bool? IsAvailable { get; set; } = true;
}