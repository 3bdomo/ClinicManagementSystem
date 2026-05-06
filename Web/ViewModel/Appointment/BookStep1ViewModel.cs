using BLL.DTOs.Patient;
using Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels.Appointment
{
    public class BookStep1ViewModel
    {
        public IEnumerable<DoctorDto> Doctors { get; set; } = new List<DoctorDto>();

        // Used only for Admin/Receptionist booking
        public IEnumerable<PatientDto> Patients { get; set; } = new List<PatientDto>();

        public Specialization? FilterSpecialization { get; set; }

        [Required(ErrorMessage = "Please select a doctor.")]
        public int? SelectedDoctorId { get; set; }

        // Required only for Admin/Receptionist.
        // Patient role will be resolved from logged-in user in controller.
        public int? SelectedPatientId { get; set; }

        public bool CanSelectPatient { get; set; }
    }
}