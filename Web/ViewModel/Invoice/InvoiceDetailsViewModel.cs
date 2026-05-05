using Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModel;
public class InvoiceDetailsViewModel
{
    public InvoiceRowViewModel Invoice { get; set; } = new();
    public IEnumerable<InvoiceItemDetailViewModel> Items { get; set; } = [];
    public AuditInfoViewModel AuditInfo { get; set; } = new();
}
