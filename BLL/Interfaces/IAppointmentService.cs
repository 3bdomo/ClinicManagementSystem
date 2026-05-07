using BLL.DTOs.Appointment;
using Common.Enums;
using Common.Results;

namespace BLL.Interfaces
{
    public interface IAppointmentService
    {
        Task<OperationResult<IEnumerable<AppointmentDto>>> GetAllAsync();
        Task<OperationResult<IEnumerable<AppointmentDto>>> GetAllAsync(int pageNumber, int pageSize);
        Task<OperationResult<AppointmentDto>> GetByIdAsync(int id);

        Task<OperationResult<IEnumerable<AppointmentDto>>> GetByDateAsync(DateTime date);

        Task<OperationResult<IEnumerable<AppointmentDto>>> GetDoctorAppointmentsAsync(int doctorId);
        Task<OperationResult<IEnumerable<AppointmentDto>>> GetDoctorAppointmentsByDateAsync(int doctorId, DateTime date);

        Task<OperationResult<IEnumerable<AppointmentDto>>> GetAppointmentsByStatusAsync(AppointmentStatus status);
        Task<OperationResult<IEnumerable<AppointmentDto>>> GetAppointmentsByTypeAsync(AppointmentType type);

        Task<OperationResult<IEnumerable<AppointmentHistoryDto>>> GetPatientHistoryAsync(int patientId);
        Task<OperationResult<IEnumerable<AppointmentHistoryDto>>> GetDoctorHistoryAsync(
            int doctorId,
            int pageNumber,
            int pageSize);

        Task<OperationResult<int>> CreateAsync(CreateAppointmentDto dto);
        Task<OperationResult> UpdateAsync(UpdateAppointmentDto dto);
        Task<OperationResult> DeleteAsync(int id);

        Task<OperationResult> StartAppointmentAsync(int id);
        Task<OperationResult> CompleteAppointmentAsync(int id);
        Task<OperationResult> CancelAppointmentAsync(int id, string? cancellationReason = null);

        
        Task<bool> IsTimeSlotAvailableAsync(
            int doctorId,
            DateTime appointmentDate,
            int durationMinutes,
            int? excludeAppointmentId = null);

        Task<bool> HasConflictingAppointmentsAsync(
            int doctorId,
            DateTime startDate,
            DateTime endDate);

        Task<int> GetTodayAppointmentsCountAsync();
    }
}