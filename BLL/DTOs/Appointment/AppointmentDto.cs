using Common.Enums;

namespace BLL.DTOs.Appointment
{
    public class AppointmentDto
    {
        public int Id { get; set; }

        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public int? DoctorScheduleId { get; set; }

        public string? DoctorName { get; set; }
        public string? PatientName { get; set; }
        public Specialization? DoctorSpecialization { get; set; }

        public DateTime AppointmentDate { get; set; }
        public int DurationMinutes { get; set; }

        public AppointmentType AppointmentType { get; set; }
        public AppointmentStatus Status { get; set; }

        public string? Notes { get; set; }
        public string? CancellationReason { get; set; }

        public bool HasMedicalRecord { get; set; }
        public bool HasInvoice { get; set; }
        public int? MedicalRecordId { get; set; }
        public int? InvoiceId { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}