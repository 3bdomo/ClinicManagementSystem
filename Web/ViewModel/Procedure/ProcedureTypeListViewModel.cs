using Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModel;
public class ProcedureTypeListViewModel
{
    public IEnumerable<ProcedureTypeFormViewModel> Types { get; set; } = [];
    public int TotalProceduresToday { get; set; }
    public decimal TotalRevenueToday { get; set; }
    public int ActiveTypesCount { get; set; }
}
