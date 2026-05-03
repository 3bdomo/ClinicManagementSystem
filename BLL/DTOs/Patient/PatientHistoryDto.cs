using BLL.DTOs.Appointment;
using BLL.DTOs.Invoice;
using BLL.DTOs.MedicalRecord;

namespace BLL.DTOs.Patient;

public class PatientHistoryDto
{
    public PatientDto Patient { get; set; } = null!;
    public IEnumerable<AppointmentDto> Appointments { get; set; } = new List<AppointmentDto>();
    public IEnumerable<MedicalRecordDto> MedicalRecords { get; set; } = new List<MedicalRecordDto>();
    public IEnumerable<InvoiceDto> Invoices { get; set; } = new List<InvoiceDto>();
}
