using Common.Enums;

namespace BLL.DTOs.Billing
{
    public class InvoiceItemDto
    {
         public int Id { get; set; }
        public int InvoiceId { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total => Quantity * UnitPrice;
        public InvoiceItemType ItemType { get; set; }
    }
}

