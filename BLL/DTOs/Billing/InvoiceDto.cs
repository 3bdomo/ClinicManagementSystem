using Common.Enums;

namespace BLL.DTOs.Billing
{
    public class InvoiceDto
    {
         public int Id { get; set; }
        public int PatientId { get; set; }
        public string? PatientName { get; set; }
        public int AppointmentId { get; set; }
        public decimal TotalAmount { get; set; }
        public InvoiceStatus Status { get; set; }
        public DateTime? PaidAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ICollection<InvoiceItemDto> Items { get; set; } = new List<InvoiceItemDto>();
    }
}

