using System.ComponentModel.DataAnnotations;
using Common.Enums;

namespace BLL.DTOs.Billing
{
    public class CreateInvoiceItemDto
    {
        [Required(ErrorMessage = "Description is required.")]
        [MaxLength(500, ErrorMessage = "Description must be at most 500 characters.")]
        public string Description { get; set; } = string.Empty;
 
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }
 
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0.")]
        public decimal UnitPrice { get; set; }
 
        [Required(ErrorMessage = "Item type is required.")]
        public InvoiceItemType ItemType { get; set; }
    }
}

