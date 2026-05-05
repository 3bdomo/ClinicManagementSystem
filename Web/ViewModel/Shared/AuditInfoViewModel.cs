using Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModel;
public class AuditInfoViewModel
{
    public string? CreatedByName { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedByName { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
