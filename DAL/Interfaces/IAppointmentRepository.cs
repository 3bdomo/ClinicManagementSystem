using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClinicSystem.DAL.Models;
namespace DAL.Interfaces
{
    public interface IAppointmentRepository: IGenericRepository<Appointment>
    {
        Task<IEnumerable<Appointment>> GetByDoctorAndDateAsync(int doctorId, DateTime date);
        Task<IEnumerable<Appointment>> GetByPatientAsync(int patientId);
        Task<IEnumerable<Appointment>> GetTodayAsync();

        Task<bool> HasConflictAsync(int doctorId, DateTime slotStart, int durationMinutes);

    }
}
