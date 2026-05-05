namespace BLL.DTOs.Statistics;

public class DashboardStatsDto
{
    public int TotalPatients { get; set; }
    public int TodayAppointments { get; set; }
    public decimal TodayRevenue { get; set; }
    public int PendingInvoicesCount { get; set; }
    public int UpcomingFollowUps { get; set; }
}