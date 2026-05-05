using Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModel;
public class InvoiceItemFormModel
{
    [Required] public string Description { get; set; } = string.Empty;
    [Range(1, 1000)] public int Quantity { get; set; } = 1;
    [Range(0, 1000000)] public decimal UnitPrice { get; set; }
    public InvoiceItemType ItemType { get; set; }
}
