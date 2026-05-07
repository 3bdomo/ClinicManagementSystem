using BLL.DTOs.Patient;
using Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels.Appointment
{
    public class BookStep1ViewModel
    {
        public IEnumerable<DoctorDto> Doctors { get; set; } = new List<DoctorDto>();

        
        public IEnumerable<PatientDto> Patients { get; set; } = new List<PatientDto>();

        public Specialization? FilterSpecialization { get; set; }

        [Required(ErrorMessage = "Please select a doctor.")]
        public int? SelectedDoctorId { get; set; }

        
        
        public int? SelectedPatientId { get; set; }

        public bool CanSelectPatient { get; set; }
    }
}