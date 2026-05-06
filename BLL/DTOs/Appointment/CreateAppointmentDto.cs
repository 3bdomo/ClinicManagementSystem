using System.ComponentModel.DataAnnotations;
using Common.Enums;

namespace BLL.DTOs.Appointment
{
    public class CreateAppointmentDto
    {
        [Required(ErrorMessage = "Doctor is required.")]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Patient is required.")]
        public int PatientId { get; set; }

        public int? DoctorScheduleId { get; set; }

        [Required(ErrorMessage = "Appointment date is required.")]
        public DateTime AppointmentDate { get; set; }

        /*
         * The service will override this value from DoctorSchedule.SlotMinutes
         * to prevent client-side tampering.
         * Still kept here because the UI sends selected slot duration.
         */
        [Required(ErrorMessage = "Duration is required.")]
        [Range(1, 480, ErrorMessage = "Duration must be greater than zero.")]
        public int DurationMinutes { get; set; }

        [Required(ErrorMessage = "Appointment type is required.")]
        public AppointmentType AppointmentType { get; set; }

        [StringLength(500, ErrorMessage = "Notes must be at most 500 characters long.")]
        public string? Notes { get; set; }
    }
}