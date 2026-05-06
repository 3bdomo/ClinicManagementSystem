using ClinicSystem.DAL.Models;
using Common.Enums;
using DAL.Context;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

internal class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
{
    public AppointmentRepository(ClinicDbContext context) : base(context)
    {
    }

    // ─────────────────────────────────────────────
    // Helper: get start/end of a day
    // ─────────────────────────────────────────────
    private static (DateTime Start, DateTime End) GetDayRange(DateTime date)
    {
        var start = date.Date;
        var end = start.AddDays(1);
        return (start, end);
    }

    // ─────────────────────────────────────────────
    // Doctor appointments in specific day
    // Used by:
    // - Doctor day view
    // - Slot conflict checks
    // - Schedule availability calculations
    // ─────────────────────────────────────────────
    public async Task<IEnumerable<Appointment>> GetByDoctorAndDateAsync(int doctorId, DateTime date)
    {
        var (start, end) = GetDayRange(date);

        return await _dbSet
            .AsNoTracking()
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .Include(a => a.DoctorSchedule)
            .Where(a => a.DoctorId == doctorId
                     && a.AppointmentDate >= start
                     && a.AppointmentDate < end
                     && a.Status != AppointmentStatus.Cancelled)
            .OrderBy(a => a.AppointmentDate)
            .ToListAsync();
    }

    // ─────────────────────────────────────────────
    // Patient appointment history
    // Used by Patient profile/history
    // ─────────────────────────────────────────────
    public async Task<IEnumerable<Appointment>> GetByPatientAsync(int patientId)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(a => a.Doctor)
            .Include(a => a.MedicalRecord)
            .Include(a => a.Invoice)
            .Where(a => a.PatientId == patientId)
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync();
    }

    // ─────────────────────────────────────────────
    // Today's non-cancelled appointments
    // Used by Dashboard
    // ─────────────────────────────────────────────
    public async Task<IEnumerable<Appointment>> GetTodayAsync()
    {
        var (start, end) = GetDayRange(DateTime.Today);

        return await _dbSet
            .AsNoTracking()
            .Include(a => a.Doctor)
            .Include(a => a.Patient)
            .Where(a => a.AppointmentDate >= start
                     && a.AppointmentDate < end
                     && a.Status != AppointmentStatus.Cancelled)
            .OrderBy(a => a.AppointmentDate)
            .ToListAsync();
    }

    // ─────────────────────────────────────────────
    // Conflict check
    // Overlap rule:
    // existingStart < newEnd && existingEnd > newStart
    // Cancelled / Completed appointments do not block slots
    // ─────────────────────────────────────────────
    public async Task<bool> HasConflictAsync(
        int doctorId,
        DateTime slotStart,
        int durationMinutes)
    {
        var slotEnd = slotStart.AddMinutes(durationMinutes);

        return await _dbSet
            .AsNoTracking()
            .AnyAsync(a => a.DoctorId == doctorId
                        && a.Status != AppointmentStatus.Cancelled
                        && a.Status != AppointmentStatus.Completed
                        && a.AppointmentDate < slotEnd
                        && a.AppointmentDate.AddMinutes(a.DurationMinutes) > slotStart);
    }

    // ─────────────────────────────────────────────
    // All appointments for doctor
    // Used by doctor history / doctor dashboard
    // ─────────────────────────────────────────────
    public async Task<IEnumerable<Appointment>> GetByDoctorAsync(int doctorId)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(a => a.Patient)
            .Include(a => a.MedicalRecord)
            .Include(a => a.Invoice)
            .Where(a => a.DoctorId == doctorId)
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync();
    }

    // ─────────────────────────────────────────────
    // Paged doctor appointments
    // ─────────────────────────────────────────────
    public async Task<IEnumerable<Appointment>> GetByDoctorAsync(
        int doctorId,
        int pageNumber,
        int pageSize)
    {
        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        pageSize = pageSize <= 0 ? 10 : pageSize;

        return await _dbSet
            .AsNoTracking()
            .Include(a => a.Patient)
            .Include(a => a.MedicalRecord)
            .Include(a => a.Invoice)
            .Where(a => a.DoctorId == doctorId)
            .OrderByDescending(a => a.AppointmentDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    // ─────────────────────────────────────────────
    // Full appointment details
    // Used by Appointment Details page
    // ─────────────────────────────────────────────
    public async Task<Appointment?> GetFullAsync(int id)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(a => a.Doctor)
            .Include(a => a.Patient)
            .Include(a => a.DoctorSchedule)
            .Include(a => a.MedicalRecord)
            .Include(a => a.Invoice)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    // ─────────────────────────────────────────────
    // All appointments in specific day
    // Used by Admin/Receptionist Index day view
    // Note:
    // We do NOT exclude Cancelled here because Index may need
    // to display/filter cancelled appointments.
    // ─────────────────────────────────────────────
    public async Task<IEnumerable<Appointment>> GetByDateAsync(DateTime date)
    {
        var (start, end) = GetDayRange(date);

        return await _dbSet
            .AsNoTracking()
            .Include(a => a.Doctor)
            .Include(a => a.Patient)
            .Include(a => a.DoctorSchedule)
            .Include(a => a.MedicalRecord)
            .Include(a => a.Invoice)
            .Where(a => a.AppointmentDate >= start
                     && a.AppointmentDate < end)
            .OrderBy(a => a.AppointmentDate)
            .ToListAsync();
    }

    // ─────────────────────────────────────────────
    // Count today's non-cancelled appointments
    // Used by dashboard stats
    // ─────────────────────────────────────────────
    public async Task<int> GetTodayCountAsync()
    {
        var (start, end) = GetDayRange(DateTime.Today);

        return await _dbSet
            .AsNoTracking()
            .CountAsync(a => a.AppointmentDate >= start
                          && a.AppointmentDate < end
                          && a.Status != AppointmentStatus.Cancelled);
    }
}