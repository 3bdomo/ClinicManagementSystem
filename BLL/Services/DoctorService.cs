using BLL.DTOs.Patient;
using BLL.Interfaces;
using Common.Enums;
using Common.Results;

namespace BLL.Services;

public class DoctorService:IDoctorService
{
    public Task<OperationResult<IEnumerable<DoctorDto>>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<DoctorDto>> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<IEnumerable<DoctorDto>>> GetBySpecializationAsync(Specialization Specialization)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult> UpdateAsync(DoctorDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult> CreateAsync(DoctorDto dto)
    {
        throw new NotImplementedException();
    }
}