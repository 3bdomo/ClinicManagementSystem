using Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModel;
public class DailyReportViewModel
{
    public DateTime SelectedDate { get; set; } = DateTime.Today;
    public decimal TotalRevenue { get; set; }
    public int TotalInvoices { get; set; }
    public int PaidCount { get; set; }
    public int UnpaidCount { get; set; }
    public int TotalProcedures { get; set; }
    public int TotalConsultations { get; set; }
    public decimal UnpaidAmount { get; set; }
}
