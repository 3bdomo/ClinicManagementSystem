using Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModel;
public class DashboardViewModel
{
    public int TotalPatients { get; set; }
    public int TodayAppointments { get; set; }
    public decimal TodayRevenue { get; set; }
    public int PendingInvoicesCount { get; set; }
    public int UpcomingFollowUps { get; set; }
    public IEnumerable<AppointmentRowViewModel> RecentAppointments { get; set; } = [];
}
