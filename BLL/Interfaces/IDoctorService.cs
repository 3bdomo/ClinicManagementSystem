using BLL.DTOs.Patient;
using Common.Enums;
using Common.Results;

namespace BLL.Interfaces;

public interface IDoctorService
{
    Task<OperationResult<IEnumerable<DoctorDto>>> GetAllAsync(int pageNumber, int pageSize);
    Task<OperationResult<DoctorDto>> GetByIdAsync(int id);

    Task<OperationResult<IEnumerable<DoctorDto>>> GetBySpecializationAsync(Specialization Specialization,
        int pageNumber, int pageSize);

    Task<OperationResult> UpdateAsync(DoctorDto dto);
    Task<OperationResult> DeleteAsync(int id);
    Task<OperationResult> CreateAsync(DoctorDto dto);
}