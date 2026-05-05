using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.MedicalRecord
{
    public class UpdateMedicalRecordDto
    {
        [Required(ErrorMessage = "Medical Record Id is required.")]
        public int Id { get; set; }
        [MinLength(10, ErrorMessage = "Diagnosis must be at least 10 characters long.")]
        [MaxLength(1000, ErrorMessage = "Diagnosis must be at most 1000 characters long.")]
        public string? Diagnosis { get; set; }
        [MaxLength(2000, ErrorMessage = "Notes must be at most 2000 characters long.")]
        public string? Notes { get; set; }
        public DateTime? FollowUpDate { get; set; }
    }
}

