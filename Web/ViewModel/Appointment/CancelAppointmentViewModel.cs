using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels.Appointment
{
    public class CancelAppointmentViewModel
    {
        public int AppointmentId { get; set; }

        public string DoctorName { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }

        [MaxLength(500, ErrorMessage = "Reason must be at most 500 characters.")]
        public string? CancellationReason { get; set; }
    }
}