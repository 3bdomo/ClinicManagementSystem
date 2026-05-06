using BLL.DTOs.Patient;
using BLL.Interfaces;
using Common.Results;
using DAL.Interfaces;

namespace BLL.Services;

public class PatientAccountService:IPatientAccountService
{
    private readonly IUnitOfWork _unitOfWork;
    public PatientAccountService(IUnitOfWork unitOfWork)
    {       
        _unitOfWork = unitOfWork;
    }

    public Task<OperationResult<string>> RegisterAsync(PatientRegisterDto dto)
    {
        var patiant = _unitOfWork.Patients.GetByNationalIdAsync(dto.NationalId).Result;
        if (patiant != null)
        {
            return Task.FromResult(
                OperationResult<string>.Failure("A patient with the same National ID already exists."));
        }

        try
        {
            _unitOfWork.Patients.AddAsync(new ClinicSystem.DAL.Models.Patient
            {
                FullName = dto.FullName,
                NationalId = dto.NationalId,
                DateOfBirth = dto.DateOfBirth,
                Gender = dto.Gender,
                Phone = dto.Phone,
                Address = dto.Address,
                BloodType = dto.BloodType,
                EmergencyContact = dto.EmergencyContact
            });
            _unitOfWork.SaveChangesAsync();
            Console.WriteLine("Patient registered successfully."+_unitOfWork.Patients.GetByNationalIdAsync(dto.NationalId).Result.FullName);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Task.FromResult(OperationResult<string>.Failure(e.Message));
            throw;
        }
       
     
        
        return Task.FromResult(OperationResult<string>.Success("Patient registered successfully."));
    }

    public Task<OperationResult<PatientDto>> GetMyProfileAsync(string userId)
    {
        var patient = _unitOfWork.Patients.GetByUserIdAsync(userId).Result;
        if (patient == null)
        {
            return Task.FromResult(OperationResult<PatientDto>.Failure("Patient not found."));
        }

        var patientDto = new PatientDto
        {
            Id = patient.Id,
            FullName = patient.FullName,
            NationalId = patient.NationalId,
            DateOfBirth = patient.DateOfBirth,
            Gender = patient.Gender,
            Phone = patient.Phone,
            Address = patient.Address,
            BloodType = patient.BloodType,
            EmergencyContact = patient.EmergencyContact
        };
        return Task.FromResult(OperationResult<PatientDto>.Success(patientDto));
    }
}