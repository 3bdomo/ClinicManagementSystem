using ClinicSystem.DAL.Models;
using Common.Enums;
using DAL.Context;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    internal class DoctorScheduleRepository: GenericRepository<DoctorSchedule>, IDoctorScheduleRepository
    {
        public DoctorScheduleRepository(ClinicDbContext context) : base(context)
        {
        }

        

        public async Task<IEnumerable<DoctorSchedule>> GetActiveByDoctorAsync(int doctorId)
           => await _dbSet
        .Where(ds => ds.DoctorId == doctorId && ds.IsActive) 
        .ToListAsync();

        public async Task<IEnumerable<DoctorSchedule>> GetByDoctorAndTypeAsync(int doctorId, ScheduleType type)
            => await _dbSet
                .Where(ds => ds.DoctorId == doctorId && ds.ScheduleType == type)
                .ToListAsync();

        public async Task<IEnumerable<DoctorSchedule>> GetByDoctorAsync(int doctorId)
            => await _dbSet
                .Where(ds => ds.DoctorId == doctorId)
                .ToListAsync();

        public async Task<DoctorSchedule?> GetScheduleForSlotAsync(int doctorId, DateTime dateTime, ScheduleType type)
        {
            // Lookup schedule for the given date (either specific date or weekly day)
            // Note: caller (GetAvailableSlotsAsync) passes the date (time may be midnight),
            // so do not require the time-of-day to be within StartTime/EndTime here —
            // we'll use StartTime/EndTime later when generating slots.
            var dateOnly = DateOnly.FromDateTime(dateTime);
            var dayOfWeek = dateTime.DayOfWeek;

            return await _dbSet
                .Where(ds => ds.DoctorId == doctorId
                          && ds.ScheduleType == type
                          && (
                                (ds.SpecificDate.HasValue && ds.SpecificDate.Value == dateOnly)
                             || (ds.DayOfWeek.HasValue && ds.DayOfWeek.Value == dayOfWeek)
                             )
                          && ds.IsActive)
                .OrderBy(ds => ds.StartTime)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> HasTimeConflictAsync(
            int doctorId,
            DayOfWeek? day,
            DateOnly? specificDate,
            TimeOnly start,
            TimeOnly end,
            ScheduleType scheduleType,
            int? excludeId = null)
        {
            return await _dbSet
                .Where(ds => ds.DoctorId == doctorId
                          && ds.ScheduleType == scheduleType
                          && (
                                (ds.SpecificDate.HasValue && specificDate.HasValue && ds.SpecificDate.Value == specificDate.Value)
                             || (ds.DayOfWeek.HasValue && day.HasValue && ds.DayOfWeek.Value == day.Value)
                             )
                          && (excludeId == null || ds.Id != excludeId)
                          && (ds.StartTime < end && ds.EndTime > start))
                .AnyAsync();
        }
    }
}
