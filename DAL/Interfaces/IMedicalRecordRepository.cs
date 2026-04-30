using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;

namespace DAL.Interfaces
{
    public interface IMedicalRecordRepository: IGenericRepository<MedicalRecord>
    {
        Task<MedicalRecord?> GetFullAsync(int recordId);
        Task<IEnumerable<MedicalRecord>> GetByPatientAsync(int patientId);
        Task<MedicalRecord?> GetByAppointmentAsync(int appointmentId);
        Task<IEnumerable<MedicalRecord>> GetUpcomingFollowUpsAsync(DateTime from, DateTime to);
    }
}
