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
        Task<OperationResult<IEnumerable<PatientDto>>> GetAllAsync();

        Task<OperationResult<IEnumerable<PatientDto>>> GetAllAsync(int page, int pageSize);
        Task<OperationResult<PatientDto>> GetByIdAsync(int id);
        Task<OperationResult<IEnumerable<PatientDto>>> SearchAsync(string query);
        Task<OperationResult> CreateAsync(PatientDto dto);
        Task<OperationResult> UpdateAsync(PatientDto dto);
        Task<OperationResult> DeleteAsync(int id);         
        Task<OperationResult<PatientHistoryDto>> GetFullHistoryAsync(int id);
        Task<OperationResult<IEnumerable<PatientDto>>> GetDeletedAsync();
        Task<OperationResult> RestoreAsync(int id);    

        Task<OperationResult<int>> GetPatientIdByApplicationUserIdAsync(string applicationUserId);
    }
}
