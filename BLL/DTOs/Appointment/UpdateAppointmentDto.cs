using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.Appointment
{
    public class UpdateAppointmentDto
    {
        [Required(ErrorMessage = "Appointment id is required.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Appointment date is required.")]
        public DateTime AppointmentDate { get; set; }

        /*
         * The service will override this value from DoctorSchedule.SlotMinutes
         * if the appointment date/time changes.
         */
        [Required(ErrorMessage = "Duration is required.")]
        [Range(1, 480, ErrorMessage = "Duration must be greater than zero.")]
        public int DurationMinutes { get; set; }

        [MaxLength(500, ErrorMessage = "Notes must be at most 500 characters long.")]
        public string? Notes { get; set; }
    }
}