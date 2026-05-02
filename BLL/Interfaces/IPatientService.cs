using BLL.DTOs.Patient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Results;
namespace BLL.Interfaces
{
    public interface IPatientService
    {
        Task<OperationResult<IEnumerable<PatientDto>>> GetAllAsync(int page, int pageSize);
        Task<OperationResult<PatientDto>> GetByIdAsync(int id);
        Task<OperationResult<IEnumerable<PatientDto>>> SearchAsync(string query);
        Task<OperationResult> CreateAsync(PatientDto dto);
        Task<OperationResult> UpdateAsync(PatientDto dto);
        Task<OperationResult> DeleteAsync(int id);         // soft delete
        Task<OperationResult<PatientHistoryDto>> GetFullHistoryAsync(int id);
        Task<OperationResult<IEnumerable<PatientDto>>> GetDeletedAsync();// ✅ Admin only
        Task<OperationResult> RestoreAsync(int id);     //     ✅ Admin only
    }
}
