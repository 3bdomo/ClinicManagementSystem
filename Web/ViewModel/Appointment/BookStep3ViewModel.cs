using BLL.DTOs.Doctor;
using Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels.Appointment
{
    public class BookStep3ViewModel
    {
        public int DoctorId { get; set; }

        public int? PatientId { get; set; }
        public string? PatientName { get; set; }

        public string DoctorName { get; set; } = string.Empty;
        public Specialization DoctorSpecialization { get; set; }

        public DateTime SelectedDate { get; set; }
        public ScheduleType ScheduleType { get; set; }

        public decimal ConsultationFee { get; set; }

        public IEnumerable<TimeSlotDto> MorningSlots { get; set; } = new List<TimeSlotDto>();
        public IEnumerable<TimeSlotDto> AfternoonSlots { get; set; } = new List<TimeSlotDto>();

        [Required(ErrorMessage = "Please select a time slot.")]
        public DateTime? SelectedSlotStart { get; set; }

        [Range(1, 480, ErrorMessage = "Invalid slot duration.")]
        public int SlotDurationMinutes { get; set; }
    }
}