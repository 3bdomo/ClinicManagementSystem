using BLL.DTOs.Patient;
using BLL.Interfaces;
using Common.Enums;
using Common.Results;
using DAL.Repositories;

namespace BLL.Services;

public class DoctorService:IDoctorService
{
    private readonly UnitOfWork _unitOfWork;
    public DoctorService(UnitOfWork unitOfWork)
    {
        this._unitOfWork = unitOfWork;
    }
    public async Task<OperationResult<IEnumerable<DoctorDto>>> GetAllAsync(int pageNumber, int pageSize)
    {
        var doctors = await _unitOfWork.Doctors.GetAllAsync(pageNumber, pageSize);

        return OperationResult<IEnumerable<DoctorDto>>.Success(
            doctors.Select(d => new DoctorDto
            {
                Id = d.Id,
                FullName = d.FullName,
                ApplicationUserId = d.ApplicationUserId,
                Bio = d.Bio,
                ConsultationFee = d.ConsultationFee,
                IsAvailable = d.IsAvailable,
                Specialization = d.Specialization
            }));
    }

    public async Task<OperationResult<DoctorDto>> GetByIdAsync(int id)
    {
        var doc = await _unitOfWork.Doctors.GetByIdAsync(id);
        if (doc != null)
        {
            return OperationResult<DoctorDto>.Success(new DoctorDto
            {
                Id = doc.Id,
                FullName = doc.FullName,
                ApplicationUserId = doc.ApplicationUserId,
                Bio = doc.Bio,
                ConsultationFee = doc.ConsultationFee,
                IsAvailable = doc.IsAvailable,
                Specialization = doc.Specialization
            });
            
        }
        return OperationResult<DoctorDto>.Failure("Doctor not found.");
    }

    public async Task<OperationResult<IEnumerable<DoctorDto>>> GetBySpecializationAsync(Specialization specialization,int pageNumber, int pageSize)
    {
        var doctors = await _unitOfWork.Doctors.GetBySpecializationAsync(specialization, pageNumber, pageSize);

        return OperationResult<IEnumerable<DoctorDto>>.Success(
            doctors.Select(d => new DoctorDto
                {
                    Id = d.Id,
                    FullName = d.FullName,
                    ApplicationUserId = d.ApplicationUserId,
                    Bio = d.Bio,
                    ConsultationFee = d.ConsultationFee,
                    IsAvailable = d.IsAvailable,
                    Specialization = d.Specialization
                }));
    }

    public async Task<OperationResult> UpdateAsync(DoctorDto dto)
    {
        var doc = await _unitOfWork.Doctors.GetByIdAsync(dto.Id);
        if (doc != null)
        {
            doc.FullName = dto.FullName;
            doc.ApplicationUserId = dto.ApplicationUserId;
            doc.Bio = dto.Bio;
            doc.ConsultationFee = dto.ConsultationFee ??doc.ConsultationFee;
            doc.IsAvailable = dto.IsAvailable ?? doc.IsAvailable;
            doc.Specialization = dto.Specialization;
            _unitOfWork.Doctors.Update(doc);
            await _unitOfWork.SaveChangesAsync();
            return OperationResult.Success();
        }
        return OperationResult.Failure("Doctor not found.");
    }

    public async Task<OperationResult> DeleteAsync(int id)
    {
        var doc = await _unitOfWork.Doctors.GetByIdAsync(id);
        if (doc != null)
        {
            _unitOfWork.Doctors.Delete(doc);
            await _unitOfWork.SaveChangesAsync();
            return OperationResult.Success();
        }
        return OperationResult.Failure("Doctor not found.");
    }

    public async Task<OperationResult> CreateAsync(DoctorDto dto)
    {
        try
        {
            var doc=new ClinicSystem.DAL.Models.Doctor
            {
                FullName = dto.FullName,
                ApplicationUserId = dto.ApplicationUserId,
                Bio = dto.Bio,
                ConsultationFee = dto.ConsultationFee ?? 0,
                IsAvailable = dto.IsAvailable ?? true,
                Specialization = dto.Specialization
            };
            await _unitOfWork.Doctors.AddAsync(doc);
            await _unitOfWork.SaveChangesAsync();
            return OperationResult.Success();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return OperationResult.Failure(e.Message);
        }
      
    }
}