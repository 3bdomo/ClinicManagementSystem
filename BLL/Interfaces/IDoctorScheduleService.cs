using BLL.DTOs.Patient;
using Common.Enums;
using Common.Results;

namespace BLL.Interfaces;

public interface IDoctorScheduleService
{
    Task<OperationResult<IEnumerable<DoctorScheduleDto>>> GetByDoctorAsync(int doctorId);
    Task<OperationResult<IEnumerable<DoctorScheduleDto>>> GetByDoctorAndTypeAsync(int doctorId, ScheduleType type);
    Task<OperationResult<DoctorScheduleDto>> GetByIdAsync(int id);
    Task<OperationResult<int>> CreateAsync(DoctorScheduleDto dto);
    Task<OperationResult> UpdateAsync(DoctorScheduleDto dto);
    Task<OperationResult> ToggleActiveAsync(int id);
    Task<OperationResult> DeleteAsync(int id);

    Task<OperationResult<DoctorScheduleDto?>> GetScheduleForSlotAsync(int doctorId, DateTime dateTime,
        ScheduleType type);

    Task<OperationResult<IEnumerable<TimeSlotDto>>> GetAvailableSlotsAsync(int doctorId, DateTime date,
        ScheduleType type);

}