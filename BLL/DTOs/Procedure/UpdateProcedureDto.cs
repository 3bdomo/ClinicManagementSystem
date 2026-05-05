using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.Procedure
{
    public class UpdateProcedureDto
    {
        [Required(ErrorMessage = "Procedure Id is required.")]
        public int Id { get; set; }
 
        public int? DurationMinutes { get; set; }
 
        [MaxLength(2000, ErrorMessage = "Notes must be at most 2000 characters.")]
        public string? Notes { get; set; }
 
        [MaxLength(2000, ErrorMessage = "After-care notes must be at most 2000 characters.")]
        public string? AfterCareNotes { get; set; }
 
        [Range(0, double.MaxValue, ErrorMessage = "Cost must be a non-negative value.")]
        public decimal? Cost { get; set; }
    }
}

