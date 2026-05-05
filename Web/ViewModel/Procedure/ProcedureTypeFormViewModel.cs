using Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModel;
public class ProcedureTypeFormViewModel
{
    public int Id { get; set; }
    [Required] public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    [Range(0, 1000000)] public decimal DefaultCost { get; set; }
    public bool IsActive { get; set; } = true;
}
