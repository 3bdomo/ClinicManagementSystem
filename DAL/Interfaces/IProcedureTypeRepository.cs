using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClinicSystem.DAL.Models;

namespace DAL.Interfaces
{
    public interface IProcedureTypeRepository: IGenericRepository<ProcedureType>
    {
        Task<IEnumerable<ProcedureType>> GetActiveAsync();
    }
}
