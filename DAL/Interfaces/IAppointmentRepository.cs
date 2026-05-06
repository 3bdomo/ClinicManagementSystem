using ClinicSystem.DAL.Models;
using Common.Enums;

namespace DAL.Interfaces;

public interface IAppointmentRepository : IGenericRepository<Appointment>
{
    Task<IEnumerable<Appointment>> GetByDoctorAndDateAsync(int doctorId, DateTime date);
    Task<IEnumerable<Appointment>> GetByPatientAsync(int patientId);
    Task<IEnumerable<Appointment>> GetTodayAsync();

    Task<bool> HasConflictAsync(int doctorId, DateTime slotStart, int durationMinutes);

    Task<IEnumerable<Appointment>> GetByDoctorAsync(int doctorId);
    Task<IEnumerable<Appointment>> GetByDoctorAsync(int doctorId, int pageNumber, int pageSize);

    Task<Appointment?> GetFullAsync(int id);

    Task<IEnumerable<Appointment>> GetByDateAsync(DateTime date);

    Task<int> GetTodayCountAsync();
}