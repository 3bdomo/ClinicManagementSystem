using BLL.DTOs.Patient;
using Common.Results;

namespace BLL.Interfaces;

public interface IPatientAccountService
{

    Task<OperationResult<string>> RegisterAsync(PatientRegisterDto dto);
    Task<OperationResult<PatientDto>> GetMyProfileAsync(string userId);
}