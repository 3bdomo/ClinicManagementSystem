using BLL.DTOs.Doctor;
using Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels.Appointment
{
    public class BookStep2ViewModel
    {
        public int DoctorId { get; set; }

        public int? PatientId { get; set; }
        public string? PatientName { get; set; }

        public string DoctorName { get; set; } = string.Empty;
        public Specialization DoctorSpecialization { get; set; }
        public string? DoctorBio { get; set; }
        public decimal ConsultationFee { get; set; }

        public IEnumerable<DoctorScheduleDto> Schedules { get; set; } = new List<DoctorScheduleDto>();

        [Required(ErrorMessage = "Please select a date.")]
        public DateTime? SelectedDate { get; set; }

        [Required(ErrorMessage = "Please select appointment type.")]
        public ScheduleType SelectedScheduleType { get; set; } = ScheduleType.Consultation;
    }
}