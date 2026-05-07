using Common.Enums;

namespace Web.ViewModel
{
    public class AppointmentRowViewModel
    {
        public int Id { get; set; }

        public int DoctorId { get; set; }
        public int PatientId { get; set; }

        public string DoctorName { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;

        public DateTime AppointmentDate { get; set; }
        public int DurationMinutes { get; set; }

        public AppointmentType AppointmentType { get; set; }
        public AppointmentStatus Status { get; set; }

        public string? Notes { get; set; }

        public bool HasMedicalRecord { get; set; }
        public bool HasInvoice { get; set; }

        public int? MedicalRecordId { get; set; }
        public int? InvoiceId { get; set; }
    }
}