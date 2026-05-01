using ClinicSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace DAL.Interfaces
{
    public interface IPatientRepository: IGenericRepository<Patient>
    {

        Task<IEnumerable<Patient>> SearchAsync(string query);
        Task<Patient?> GetWithFullHistoryAsync(int patientId);
        Task<IEnumerable<Patient>> GetDeletedAsync();    
        Task<bool> RestoreAsync(int patientId);
        Task<Patient?> GetByNationalIdAsync(string nationalId);
        Task<Patient?> GetByUserIdAsync(int userId);
    }
}
