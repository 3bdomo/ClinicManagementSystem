using System.ComponentModel.DataAnnotations;
using Common.Enums;

namespace BLL.DTOs.Appointment
{
    public class UpdateAppointmentDto
    {
        [Required(ErrorMessage = "Appointment Id is required.")]
        public int Id { get; set; }
        
        public DateTime AppointmentDate { get; set; }
        [Required(ErrorMessage = "DurationMinutes is required.")]
        [Range(15, 45, ErrorMessage = "DurationMinutes must be between 15 and 45 minutes.")]   
        public int DurationMinutes { get; set; }
        
        public AppointmentType? AppointmentType { get; set; }
        [MaxLength(500,ErrorMessage = "Notes must be at most 500 characters long.")]
        public string? Notes { get; set; }
    }
}

