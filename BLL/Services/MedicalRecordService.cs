using AutoMapper;
using BLL.DTOs.MedicalRecord;
using BLL.Interfaces;
using ClinicSystem.DAL.Models;
using Common.Results;
using DAL.Interfaces;

namespace BLL.Services
{
    
        public class MedicalRecordService : IMedicalRecordService
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;

            public MedicalRecordService(IUnitOfWork unitOfWork, IMapper mapper)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }

            public async Task<OperationResult<IEnumerable<MedicalRecordDto>>> GetAllAsync()
            {
                var records = await _unitOfWork.MedicalRecords.GetAllAsync(1, int.MaxValue);
                var dtos = _mapper.Map<IEnumerable<MedicalRecordDto>>(records);
                return OperationResult<IEnumerable<MedicalRecordDto>>.Success(dtos);
            }

            public async Task<OperationResult<IEnumerable<MedicalRecordDto>>> GetAllAsync(int pageNumber, int pageSize)
            {
                var records = await _unitOfWork.MedicalRecords.GetAllAsync(pageNumber, pageSize);
                var dtos = _mapper.Map<IEnumerable<MedicalRecordDto>>(records);
                return OperationResult<IEnumerable<MedicalRecordDto>>.Success(dtos);
            }

            public async Task<OperationResult<MedicalRecordDto>> GetByIdAsync(int id)
            {
                var record = await _unitOfWork.MedicalRecords.GetByIdAsync(id);
                if (record is null)
                    return OperationResult<MedicalRecordDto>.Failure("Medical record not found.");

                return OperationResult<MedicalRecordDto>.Success(_mapper.Map<MedicalRecordDto>(record));
            }

            public async Task<OperationResult<MedicalRecordDto>> GetFullAsync(int id)
            {
                var record = await _unitOfWork.MedicalRecords.GetFullAsync(id);
                if (record is null)
                    return OperationResult<MedicalRecordDto>.Failure("Medical record not found.");

                var dto = _mapper.Map<MedicalRecordDto>(record);
                return OperationResult<MedicalRecordDto>.Success(dto);
            }

            public async Task<OperationResult<int>> CreateAsync(CreateMedicalRecordDto dto)
            {
                var existingRecord = await _unitOfWork.MedicalRecords.GetByAppointmentAsync(dto.AppointmentId);
                if (existingRecord is not null)
                    return OperationResult<int>.Failure("A medical record already exists for this appointment.");

                var appointment = await _unitOfWork.Appointments.GetByIdAsync(dto.AppointmentId);
                if (appointment is null)
                    return OperationResult<int>.Failure("Appointment not found.");

                var patient = await _unitOfWork.Patients.GetByIdAsync(dto.PatientId);
                if (patient is null)
                    return OperationResult<int>.Failure("Patient not found.");

                var doctor = await _unitOfWork.Doctors.GetByIdAsync(dto.DoctorId);
                if (doctor is null)
                    return OperationResult<int>.Failure("Doctor not found.");

                var record = _mapper.Map<MedicalRecord>(dto);
                await _unitOfWork.MedicalRecords.AddAsync(record);
                await _unitOfWork.SaveChangesAsync();

                return OperationResult<int>.Success(record.Id, "Medical record created successfully.");
            }

            public async Task<OperationResult> UpdateAsync(UpdateMedicalRecordDto dto)
            {
                var record = await _unitOfWork.MedicalRecords.GetByIdAsync(dto.Id);
                if (record is null)
                    return OperationResult.Failure("Medical record not found.");

                if (dto.Diagnosis is not null)
                    record.Diagnosis = dto.Diagnosis;

                if (dto.Notes is not null)
                    record.Notes = dto.Notes;

                if (dto.FollowUpDate.HasValue)
                    record.FollowUpDate = dto.FollowUpDate;

                _unitOfWork.MedicalRecords.Update(record);
                await _unitOfWork.SaveChangesAsync();

                return OperationResult.Success("Medical record updated successfully.");
            }

            public async Task<OperationResult> DeleteAsync(int id)
            {
                var record = await _unitOfWork.MedicalRecords.GetByIdAsync(id);
                if (record is null)
                    return OperationResult.Failure("Medical record not found.");

                _unitOfWork.MedicalRecords.Delete(record);
                await _unitOfWork.SaveChangesAsync();

                return OperationResult.Success("Medical record deleted successfully.");
            }

            public async Task<OperationResult<IEnumerable<MedicalRecordDto>>> GetByPatientAsync(int patientId)
            {
                var patient = await _unitOfWork.Patients.GetByIdAsync(patientId);
                if (patient is null)
                    return OperationResult<IEnumerable<MedicalRecordDto>>.Failure("Patient not found.");

                var records = await _unitOfWork.MedicalRecords.GetByPatientAsync(patientId);
                var dtos = _mapper.Map<IEnumerable<MedicalRecordDto>>(records);
                return OperationResult<IEnumerable<MedicalRecordDto>>.Success(dtos);
            }

            public async Task<OperationResult<MedicalRecordDto>> GetByAppointmentAsync(int appointmentId)
            {
                var record = await _unitOfWork.MedicalRecords.GetByAppointmentAsync(appointmentId);
                if (record is null)
                    return OperationResult<MedicalRecordDto>.Failure("No medical record found for this appointment.");

                return OperationResult<MedicalRecordDto>.Success(_mapper.Map<MedicalRecordDto>(record));
            }

            public async Task<OperationResult<IEnumerable<MedicalRecordDto>>> GetUpcomingFollowUpsAsync(DateTime from,
                DateTime to)
            {
                if (from > to)
                    return OperationResult<IEnumerable<MedicalRecordDto>>.Failure(
                        "Start date must be before end date.");

                var records = await _unitOfWork.MedicalRecords.GetUpcomingFollowUpsAsync(from, to);
                var dtos = _mapper.Map<IEnumerable<MedicalRecordDto>>(records);
                return OperationResult<IEnumerable<MedicalRecordDto>>.Success(dtos);
            }

            public async Task<OperationResult<PatientMedicalStatisticsDto>> GetPatientStatisticsAsync(int patientId)
            {
                var patient = await _unitOfWork.Patients.GetByIdAsync(patientId);
                if (patient is null)
                    return OperationResult<PatientMedicalStatisticsDto>.Failure("Patient not found.");

                var records = (await _unitOfWork.MedicalRecords.GetByPatientAsync(patientId)).ToList();
                var now = DateTime.UtcNow;

                var statistics = new PatientMedicalStatisticsDto
                {
                    PatientId = patientId,
                    TotalRecords = records.Count,
                    TotalVisits = records.Count,
                    TotalProcedures = records.Sum(r => r.Procedures?.Count ?? 0),

                    UniqueConditions = records
                        .Select(r => r.Diagnosis.ToLower().Trim())
                        .Distinct()
                        .Count(),
                    LatestDiagnosis = records
                        .OrderByDescending(r => r.VisitDate)
                        .FirstOrDefault()?.Diagnosis,
                    LastVisitDate = records
                        .OrderByDescending(r => r.VisitDate)
                        .FirstOrDefault()?.VisitDate,
                    FirstRecordDate = records
                        .OrderBy(r => r.VisitDate)
                        .FirstOrDefault()?.VisitDate,
                    PendingFollowUps = records
                        .Count(r => r.FollowUpDate.HasValue && r.FollowUpDate.Value >= now),
                    AverageVisitsPerRecord = records.Count > 0 ? 1.0 : 0.0
                };

                return OperationResult<PatientMedicalStatisticsDto>.Success(statistics);
            }
        }
}