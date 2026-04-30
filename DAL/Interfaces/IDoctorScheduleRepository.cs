using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;
namespace DAL.Interfaces
{
    public interface IDoctorScheduleRepository: IGenericRepository<DoctorSchedule>
    {
        Task<IEnumerable<DoctorSchedule>> GetByDoctorAsync(int doctorId);
        Task<IEnumerable<DoctorSchedule>> GetActiveByDoctorAsync(int doctorId);
        Task<IEnumerable<DoctorSchedule>> GetByDoctorAndTypeAsync(int doctorId, ScheduleType type);

        Task<DoctorSchedule?> GetScheduleForSlotAsync(int doctorId, DateTime dateTime, ScheduleType type);
        Task<bool> HasTimeConflictAsync(int doctorId, DayOfWeek? day, DateOnly? specificDate, TimeOnly start, TimeOnly end, ScheduleType scheduleType, int? excludeId = null);
    }
}
