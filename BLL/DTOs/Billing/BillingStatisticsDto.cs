namespace BLL.DTOs.Billing
{
    public class BillingStatisticsDto
    {
        public decimal TotalRevenue { get; set; }
        public decimal PaidRevenue { get; set; }
        public decimal UnpaidRevenue { get; set; }
        public decimal PartiallyPaidRevenue { get; set; }
        public int TotalInvoices { get; set; }
        public int PaidInvoices { get; set; }
        public int UnpaidInvoices { get; set; }
        public int PartiallyPaidInvoices { get; set; }
    }
}

