using Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels.Appointment
{
    public class BookConfirmViewModel
    {
        [Required]
        public int DoctorId { get; set; }

        [Required]
        public int PatientId { get; set; }

        public int? DoctorScheduleId { get; set; }

        [Required(ErrorMessage = "Appointment date is required.")]
        public DateTime AppointmentDate { get; set; }

        [Range(1, 480, ErrorMessage = "Invalid appointment duration.")]
        public int DurationMinutes { get; set; }

        [Required]
        public AppointmentType AppointmentType { get; set; }

        [Required]
        public ScheduleType ScheduleType { get; set; }

        [MaxLength(500, ErrorMessage = "Notes must be at most 500 characters.")]
        public string? Notes { get; set; }

        public string DoctorName { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public Specialization DoctorSpecialization { get; set; }
        public decimal ConsultationFee { get; set; }
    }
}