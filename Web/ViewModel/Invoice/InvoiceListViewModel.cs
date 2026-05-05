using Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModel;
public class InvoiceListViewModel
{
    public IEnumerable<InvoiceRowViewModel> Invoices { get; set; } = [];
    public InvoiceStatus? Status { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalCount { get; set; }
    public int PaidCount { get; set; }
    public int UnpaidCount { get; set; }
}
