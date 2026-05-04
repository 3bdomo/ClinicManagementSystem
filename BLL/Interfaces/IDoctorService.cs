using BLL.DTOs.Patient;
using Common.Enums;
using Common.Results;

namespace BLL.Interfaces;

public interface IDoctorService
{
    Task<OperationResult<IEnumerable<DoctorDto>>> GetAllAsync();
    Task<OperationResult<DoctorDto>> GetByIdAsync(int id);
    Task<OperationResult<IEnumerable<DoctorDto>>> GetBySpecializationAsync(Specialization Specialization);
    Task<OperationResult> UpdateAsync(DoctorDto dto);
    Task<OperationResult> DeleteAsync(int id);
    Task<OperationResult> CreateAsync(DoctorDto dto);
}