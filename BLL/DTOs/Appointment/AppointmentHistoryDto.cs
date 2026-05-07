using Common.Enums;

namespace BLL.DTOs.Appointment
{
    public class AppointmentHistoryDto
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public string? DoctorName { get; set; }
        public Specialization? DoctorSpecialization { get; set; }

        public DateTime AppointmentDate { get; set; }
        public int DurationMinutes { get; set; }

        public AppointmentType AppointmentType { get; set; }
        public AppointmentStatus Status { get; set; }

        public string? Notes { get; set; }
        public string? CancellationReason { get; set; }

        public bool HasMedicalRecord { get; set; }
        public bool HasInvoice { get; set; }
    }
}