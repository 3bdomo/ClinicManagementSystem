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
        Task<OperationResult<IEnumerable<PatientDTO>>> GetAllAsync(int page, int pageSize);
        Task<OperationResult<PatientDTO>> GetByIdAsync(int id);
        Task<OperationResult<IEnumerable<PatientDTO>>> SearchAsync(string query);
        Task<OperationResult> CreateAsync(PatientDTO dto);
        Task<OperationResult> UpdateAsync(PatientDTO dto);
        Task<OperationResult> DeleteAsync(int id);         // soft delete
        Task<OperationResult<PatientHistoryDto>> GetFullHistoryAsync(int id);
        Task<OperationResult<IEnumerable<PatientDTO>>> GetDeletedAsync();// ✅ Admin only
        Task<OperationResult> RestoreAsync(int id);     //     ✅ Admin only
    }
}
