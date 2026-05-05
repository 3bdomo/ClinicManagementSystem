using System.ComponentModel.DataAnnotations;
using Common.Enums;

namespace BLL.DTOs.Appointment
{

    public class CreateAppointmentDto
    {
        [Required(ErrorMessage = "DoctorId is required.")]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "PatientId is required.")]
        public int PatientId { get; set; }

        public int? DoctorScheduleId { get; set; }

        [Required(ErrorMessage = "AppointmentDate is required.")]
        public DateTime AppointmentDate { get; set; }

        [Required(ErrorMessage = "DurationMinutes is required.")]
        [Range(15, 45, ErrorMessage = "DurationMinutes must be between 15 and 45 minutes.")]   
        public int DurationMinutes { get; set; }

        [Required(ErrorMessage = "Type is required.")]
        public AppointmentType Type { get; set; }

        [StringLength(500, ErrorMessage = "Notes must be at most 500 characters long.")]
        public string? Notes { get; set; }
    }
}