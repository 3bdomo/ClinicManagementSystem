using ClinicSystem.DAL.Models;
using Common.Enums;
using DAL.Context;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

internal class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
{
    public AppointmentRepository(ClinicDbContext context) : base(context) { }

    public async Task<IEnumerable<Appointment>> GetByDoctorAndDateAsync(int doctorId, DateTime date)
    {
        var dateOnly = date.Date;
        return await _dbSet
            .Where(a => a.DoctorId == doctorId
                     && a.AppointmentDate.Date == dateOnly
                     && a.Status != AppointmentStatus.Cancelled)
            .OrderBy(a => a.AppointmentDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetByPatientAsync(int patientId)
        => await _dbSet
            .Where(a => a.PatientId == patientId)
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync();

    public async Task<IEnumerable<Appointment>> GetTodayAsync()
    {
        var today = DateTime.UtcNow.Date;
        return await _dbSet
            .Where(a => a.AppointmentDate.Date == today
                     && a.Status != AppointmentStatus.Cancelled)
            .OrderBy(a => a.AppointmentDate)
            .ToListAsync();
    }

   
    public async Task<bool> HasConflictAsync(int doctorId, DateTime slotStart, int durationMinutes)
    {
        var slotEnd = slotStart.AddMinutes(durationMinutes);

        return await _dbSet
            .AnyAsync(a => a.DoctorId == doctorId
                        && a.Status != AppointmentStatus.Cancelled
                        && a.AppointmentDate < slotEnd
                        && a.AppointmentDate.AddMinutes(a.DurationMinutes) > slotStart);
    }

    public async Task<IEnumerable<Appointment>> GetByDoctorAsync(int doctorId)
        => await _dbSet
            .Where(a => a.DoctorId == doctorId)
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync();

    public async Task<IEnumerable<Appointment>> GetByDoctorAsync(int doctorId, int pageNumber, int pageSize)
        => await _dbSet
            .Where(a => a.DoctorId == doctorId)
            .OrderByDescending(a => a.AppointmentDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
}