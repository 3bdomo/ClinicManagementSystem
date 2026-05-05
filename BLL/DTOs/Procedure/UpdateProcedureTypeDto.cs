using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.Procedure
{
    public class UpdateProcedureTypeDto
    {
        [Required(ErrorMessage = "Procedure Type Id is required.")]
        public int Id { get; set; }
 
        [MaxLength(200, ErrorMessage = "Name must be at most 200 characters.")]
        public string? Name { get; set; }
 
        [MaxLength(1000, ErrorMessage = "Description must be at most 1000 characters.")]
        public string? Description { get; set; }
 
        [Range(0, double.MaxValue, ErrorMessage = "Default cost must be a non-negative value.")]
        public decimal? DefaultCost { get; set; }
 
        public bool? IsActive { get; set; }
    }
}

