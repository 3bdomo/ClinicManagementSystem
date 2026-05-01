using Common.Enums;
using Common.Interfaces;
namespace ClinicSystem.DAL.Models;

public class InvoiceItem
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public InvoiceItemType ItemType { get; set; }

    // Navigation Properties
    public Invoice Invoice { get; set; } = null!;
}