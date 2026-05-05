using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.MedicalRecord
{
    public class CreateMedicalRecordDto
    {
        [Required(ErrorMessage = "Patient Id is required.")]
        public int PatientId { get; set; }
        [Required(ErrorMessage = "Doctor Id is required.")]
        public int DoctorId { get; set; }
        [Required(ErrorMessage = "Appointment Id is required.")]
        public int AppointmentId { get; set; }
        [Required(ErrorMessage = "Diagnosis is required.")]
        [MinLength(10, ErrorMessage = "Diagnosis must be at least 10 characters long.")]
        [MaxLength(1000, ErrorMessage = "Diagnosis must be at most 1000 characters long.")]
        public string? Diagnosis { get; set; }
        [MaxLength(2000, ErrorMessage = "Notes must be at most 2000 characters long.")]
        public string? Notes { get; set; }
        [Required(ErrorMessage = "Visited Date is required.")]
        public DateTime VisitedDate { get; set; }
        [Required(ErrorMessage = "Follow Up Date is required.")]
        public DateTime? FollowUpDate { get; set; }
    }
}

