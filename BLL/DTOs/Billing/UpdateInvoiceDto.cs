using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.Billing
{
    public class UpdateInvoiceDto
    {
        [Required(ErrorMessage = "Invoice Id is required.")]
        public int Id { get; set; }
 
        public List<CreateInvoiceItemDto>? Items { get; set; }
    }
}

