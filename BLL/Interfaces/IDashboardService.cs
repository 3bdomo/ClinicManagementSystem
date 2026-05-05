using BLL.DTOs.Statistics;
using Common.Results;

namespace BLL.Interfaces;

public interface IDashboardService
{
    Task<OperationResult<DashboardStatsDto>> GetStatsAsync();
    Task<OperationResult<TodaySummaryDto>> GetTodaySummaryAsync();
}

