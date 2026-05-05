using BLL.DTOs.Appointment;
using Common.Results;

namespace BLL.Interfaces
{
    public interface IAppointmentService
    {
       
        Task<OperationResult<IEnumerable<AppointmentDto>>> GetAllAsync();
        Task<OperationResult<IEnumerable<AppointmentDto>>> GetAllAsync(int pageNumber, int pageSize);
        Task<OperationResult<AppointmentDto>> GetByIdAsync(int id);
        Task<OperationResult<int>> CreateAsync(AppointmentDto appointmentDto);
        Task<OperationResult> UpdateAsync(AppointmentDto appointmentDto);
        Task<OperationResult> DeleteAsync(int id);
        Task<OperationResult<IEnumerable<AppointmentHistoryDto>>> GetPatientHistoryAsync(int patientId);

        Task<OperationResult<IEnumerable<AppointmentHistoryDto>>> GetDoctorHistoryAsync(int doctorId, int pageNumber,
            int pageSize);

        Task<OperationResult<IEnumerable<AppointmentDto>>> GetDoctorAppointmentsAsync(int doctorId);

        Task<OperationResult<IEnumerable<AppointmentDto>>> GetDoctorAppointmentsByDateAsync(
            int doctorId,
            DateTime date);

        Task<OperationResult<IEnumerable<AppointmentDto>>> GetAppointmentsByStatusAsync(
            Common.Enums.AppointmentStatus status);

        Task<OperationResult<IEnumerable<AppointmentDto>>> GetAppointmentsByTypeAsync(
            Common.Enums.AppointmentType type);

        Task<OperationResult> StartAppointmentAsync(int id);
        Task<OperationResult> CompleteAppointmentAsync(int id);
        Task<OperationResult> CancelAppointmentAsync(int id, string? cancellationReason = null);

        Task<bool> IsTimeSlotAvailableAsync(
            int doctorId,
            DateTime appointmentDate,
            int durationMinutes,
            int? excludeAppointmentId = null);

        Task<OperationResult<IEnumerable<DateTime>>> GetAvailableSlotsAsync(
            int doctorId,
            DateTime date,
            int slotDurationMinutes = 30);

        Task<OperationResult<IEnumerable<AppointmentDto>>> GetDeletedAsync();
        Task<OperationResult> RestoreAsync(int id);
        Task<int> GetTodayAppointmentsCountAsync();

        Task<bool> HasConflictingAppointmentsAsync(
            int doctorId,
            DateTime startDate,
            DateTime endDate);
        
    }
}

