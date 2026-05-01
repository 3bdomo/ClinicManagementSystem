using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClinicSystem.DAL.Models;

namespace DAL.Interfaces
{
    public interface IProcedureRepository: IGenericRepository<Procedure>
    {
        Task<IEnumerable<Procedure>> GetByMedicalRecordAsync(int recordId);
        Task<IEnumerable<Procedure>> GetByPatientAsync(int patientId);
    }
}
