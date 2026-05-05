using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.Procedure
{
    public class CreateProcedureDto
    {
        [Required(ErrorMessage = "Medical Record Id is required.")]
        public int MedicalRecordId { get; set; }
 
        [Required(ErrorMessage = "Procedure Type Id is required.")]
        public int ProcedureTypeId { get; set; }
 
        [Required(ErrorMessage = "Performed date is required.")]
        public DateTime PerformedAt { get; set; }
 
        [Range(1, 1440, ErrorMessage = "Duration must be between 1 and 1440 minutes.")]
        public int? DurationMinutes { get; set; }
 
        [MaxLength(2000, ErrorMessage = "Notes must be at most 2000 characters.")]
        public string? Notes { get; set; }
 
        [MaxLength(2000, ErrorMessage = "After-care notes must be at most 2000 characters.")]
        public string? AfterCareNotes { get; set; }
 
        [Range(0, double.MaxValue, ErrorMessage = "Cost must be a non-negative value.")]
        public decimal Cost { get; set; }
    }
    
}

