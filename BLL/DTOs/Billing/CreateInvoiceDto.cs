using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.Billing
{
    public class CreateInvoiceDto
    {
        [Required(ErrorMessage = "Patient Id is required.")]
        public int PatientId { get; set; }
 
        [Required(ErrorMessage = "Appointment Id is required.")]
        public int AppointmentId { get; set; }
 
        [Required(ErrorMessage = "At least one invoice item is required.")]
        [MinLength(1, ErrorMessage = "At least one invoice item is required.")]
        public List<CreateInvoiceItemDto> Items { get; set; } = new();
    }
}

