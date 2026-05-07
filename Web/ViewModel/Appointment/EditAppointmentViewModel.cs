using System.ComponentModel.DataAnnotations;
using Common.Enums;

namespace Web.ViewModels.Appointment
{
    public class EditAppointmentViewModel
    {
        public int Id { get; set; }

        public int DoctorId { get; set; }

        public string DoctorName { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;

        public AppointmentStatus CurrentStatus { get; set; }

        public AppointmentType AppointmentType { get; set; }
        public ScheduleType ScheduleType { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        public DateTime AppointmentDate { get; set; }

        /*
         * The BLL will override duration from DoctorSchedule.SlotMinutes
         * when the appointment date/time changes.
         */
        [Range(1, 480, ErrorMessage = "Duration must be greater than zero.")]
        public int DurationMinutes { get; set; }

        [MaxLength(500, ErrorMessage = "Notes must be at most 500 characters.")]
        public string? Notes { get; set; }

        
        public IEnumerable<DateTime> AvailableSlots { get; set; } = new List<DateTime>();
    }
}