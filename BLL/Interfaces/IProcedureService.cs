using BLL.DTOs.Procedure;
using Common.Results;

namespace BLL.Interfaces
{
    public interface IProcedureService
    {
      Task<OperationResult<IEnumerable<ProcedureDto>>> GetAllAsync();
        Task<OperationResult<ProcedureDto>> GetByIdAsync(int id);
        Task<OperationResult<int>> CreateAsync(CreateProcedureDto dto);
        Task<OperationResult> UpdateAsync(UpdateProcedureDto dto);
        Task<OperationResult> DeleteAsync(int id);
 
        Task<OperationResult<IEnumerable<ProcedureDto>>> GetByMedicalRecordAsync(int medicalRecordId);
        Task<OperationResult<IEnumerable<ProcedureDto>>> GetByPatientAsync(int patientId);
 
        
        Task<OperationResult<IEnumerable<ProcedureTypeDto>>> GetAllTypesAsync();
        Task<OperationResult<IEnumerable<ProcedureTypeDto>>> GetActiveTypesAsync();
        Task<OperationResult<ProcedureTypeDto>> GetTypeByIdAsync(int id);
        Task<OperationResult<int>> CreateTypeAsync(CreateProcedureTypeDto dto);
        Task<OperationResult> UpdateTypeAsync(UpdateProcedureTypeDto dto);
        Task<OperationResult> DeactivateTypeAsync(int id);
        Task<OperationResult> ActivateTypeAsync(int id);
    }
}

