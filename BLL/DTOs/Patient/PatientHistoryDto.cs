using BLL.DTOs.Appointment;



namespace BLL.DTOs.Patient;

public class PatientHistoryDto
{
    public PatientDto Patient { get; set; } = null!;
    public IEnumerable<AppointmentDto> Appointments { get; set; } = new List<AppointmentDto>();
    public BLL.DTOs.Shared.AuditInfoDto AuditInfo { get; set; } = new();
}
