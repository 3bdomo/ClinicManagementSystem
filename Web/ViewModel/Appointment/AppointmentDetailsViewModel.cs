using Common.Enums;

namespace Web.ViewModels.Appointment
{
    public class AppointmentDetailsViewModel
    {
        public int Id { get; set; }

        public string DoctorName { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public Specialization? DoctorSpecialization { get; set; }

        public DateTime AppointmentDate { get; set; }
        public int DurationMinutes { get; set; }

        public AppointmentType AppointmentType { get; set; }
        public AppointmentStatus Status { get; set; }

        public string? Notes { get; set; }
        public string? CancellationReason { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public bool HasMedicalRecord { get; set; }
        public bool HasInvoice { get; set; }
        public int? MedicalRecordId { get; set; }
        public int? InvoiceId { get; set; }

        public bool CanEdit { get; set; }
        public bool CanCancel { get; set; }
        public bool CanStart { get; set; }
        public bool CanComplete { get; set; }

        
        public bool CanDelete { get; set; }
    }
}