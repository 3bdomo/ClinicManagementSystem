using BLL.DTOs.Appointment;
using System.Collections.Generic;

namespace BLL.DTOs.Statistics;

public class TodaySummaryDto
{
    public IEnumerable<AppointmentDto> Appointments { get; set; } = new List<AppointmentDto>();
}