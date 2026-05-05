using BLL.DTOs.MedicalRecord;
using Common.Results;

namespace BLL.Interfaces
{
    public interface IMedicalRecordService
    {
        Task<OperationResult<IEnumerable<MedicalRecordDto>>> GetAllAsync();
        Task<OperationResult<IEnumerable<MedicalRecordDto>>> GetAllAsync(int pageNumber, int pageSize);
        Task<OperationResult<MedicalRecordDto>> GetByIdAsync(int id);
        Task<OperationResult<MedicalRecordDto>> GetFullAsync(int id);
        Task<OperationResult<int>> CreateAsync(CreateMedicalRecordDto dto);
        Task<OperationResult> UpdateAsync(UpdateMedicalRecordDto dto);
        Task<OperationResult> DeleteAsync(int id);
 
        Task<OperationResult<IEnumerable<MedicalRecordDto>>> GetByPatientAsync(int patientId);
        Task<OperationResult<MedicalRecordDto>> GetByAppointmentAsync(int appointmentId);
        Task<OperationResult<IEnumerable<MedicalRecordDto>>> GetUpcomingFollowUpsAsync(DateTime from, DateTime to);
        Task<OperationResult<PatientMedicalStatisticsDto>> GetPatientStatisticsAsync(int patientId);        
    }
}

