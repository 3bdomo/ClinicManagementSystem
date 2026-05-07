using BLL.DTOs.Patient;
using Common.Enums;
using Web.ViewModel;

namespace Web.ViewModels.Appointment
{
    public class AppointmentIndexViewModel
    {
        public IEnumerable<AppointmentRowViewModel> DayAppointments { get; set; }
            = new List<AppointmentRowViewModel>();

        public DateTime SelectedDate { get; set; } = DateTime.Today;

        public int CalendarMonth { get; set; }
        public int CalendarYear { get; set; }

        public IEnumerable<DateTime> BusyDays { get; set; } = new List<DateTime>();

        public int? FilterDoctorId { get; set; }
        public AppointmentStatus? FilterStatus { get; set; }
        public AppointmentType? FilterType { get; set; }

        public IEnumerable<DoctorDto> Doctors { get; set; } = new List<DoctorDto>();

        public int TotalToday { get; set; }
        public int WaitingCount { get; set; }
        public int InProgressCount { get; set; }
        public int CompletedCount { get; set; }
    }
}