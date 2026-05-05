using BLL.DTOs.Patient;
using BLL.Interfaces;
using Common.Enums;
using Common.Results;

namespace BLL.Services;

public class DoctorScheduleService:IDoctorScheduleService
{
    public Task<OperationResult<IEnumerable<DoctorScheduleDto>>> GetByDoctorAsync(int doctorId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<IEnumerable<DoctorScheduleDto>>> GetByDoctorAndTypeAsync(int doctorId, ScheduleType type)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<DoctorScheduleDto>> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<int>> CreateAsync(DoctorScheduleDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult> UpdateAsync(DoctorScheduleDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult> ToggleActiveAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<DoctorScheduleDto?>> GetScheduleForSlotAsync(int doctorId, DateTime dateTime, ScheduleType type)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<IEnumerable<TimeSlotDto>>> GetAvailableSlotsAsync(int doctorId, DateTime date, ScheduleType type)
    {
        throw new NotImplementedException();
    }
}