using AutoMapper;
using BLL.DTOs.Appointment;
using BLL.Interfaces;
using ClinicSystem.DAL.Models;
using Common.Enums;
using Common.Results;
using DAL.Interfaces;

namespace BLL.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AppointmentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<OperationResult<IEnumerable<AppointmentDto>>> GetAllAsync()
        {
            var appointments = await _unitOfWork.Appointments.GetAllAsync();

            var dtos = _mapper.Map<IEnumerable<AppointmentDto>>(appointments);

            return OperationResult<IEnumerable<AppointmentDto>>.Success(dtos);
        }

        public async Task<OperationResult<IEnumerable<AppointmentDto>>> GetAllAsync(
            int pageNumber,
            int pageSize)
        {
            var appointments = await _unitOfWork.Appointments.GetAllAsync(pageNumber, pageSize);

            var dtos = _mapper.Map<IEnumerable<AppointmentDto>>(appointments);

            return OperationResult<IEnumerable<AppointmentDto>>.Success(dtos);
        }

        public async Task<OperationResult<AppointmentDto>> GetByIdAsync(int id)
        {
            var appointment = await _unitOfWork.Appointments.GetFullAsync(id);

            if (appointment == null)
                return OperationResult<AppointmentDto>.Failure("Appointment not found.");

            var dto = _mapper.Map<AppointmentDto>(appointment);

            return OperationResult<AppointmentDto>.Success(dto);
        }

        public async Task<OperationResult<IEnumerable<AppointmentDto>>> GetByDateAsync(DateTime date)
        {
            var appointments = await _unitOfWork.Appointments.GetByDateAsync(date);

            var dtos = _mapper.Map<IEnumerable<AppointmentDto>>(appointments);

            return OperationResult<IEnumerable<AppointmentDto>>.Success(dtos);
        }

        public async Task<OperationResult<IEnumerable<AppointmentHistoryDto>>> GetPatientHistoryAsync(
            int patientId)
        {
            var appointments = await _unitOfWork.Appointments.GetByPatientAsync(patientId);

            var dtos = _mapper.Map<IEnumerable<AppointmentHistoryDto>>(appointments);

            return OperationResult<IEnumerable<AppointmentHistoryDto>>.Success(dtos);
        }

        public async Task<OperationResult<IEnumerable<AppointmentHistoryDto>>> GetDoctorHistoryAsync(
            int doctorId,
            int pageNumber,
            int pageSize)
        {
            var appointments = await _unitOfWork.Appointments.GetByDoctorAsync(
                doctorId,
                pageNumber,
                pageSize);

            var dtos = _mapper.Map<IEnumerable<AppointmentHistoryDto>>(appointments);

            return OperationResult<IEnumerable<AppointmentHistoryDto>>.Success(dtos);
        }

        public async Task<OperationResult<IEnumerable<AppointmentDto>>> GetDoctorAppointmentsAsync(
            int doctorId)
        {
            var appointments = await _unitOfWork.Appointments.GetByDoctorAsync(doctorId);

            var dtos = _mapper.Map<IEnumerable<AppointmentDto>>(appointments);

            return OperationResult<IEnumerable<AppointmentDto>>.Success(dtos);
        }

        public async Task<OperationResult<IEnumerable<AppointmentDto>>> GetDoctorAppointmentsByDateAsync(
            int doctorId,
            DateTime date)
        {
            var appointments = await _unitOfWork.Appointments.GetByDoctorAndDateAsync(
                doctorId,
                date);

            var dtos = _mapper.Map<IEnumerable<AppointmentDto>>(appointments);

            return OperationResult<IEnumerable<AppointmentDto>>.Success(dtos);
        }

        public async Task<OperationResult<IEnumerable<AppointmentDto>>> GetAppointmentsByStatusAsync(
            AppointmentStatus status)
        {
            var appointments = await _unitOfWork.Appointments.FindAsync(a => a.Status == status);

            var dtos = _mapper.Map<IEnumerable<AppointmentDto>>(appointments);

            return OperationResult<IEnumerable<AppointmentDto>>.Success(dtos);
        }

        public async Task<OperationResult<IEnumerable<AppointmentDto>>> GetAppointmentsByTypeAsync(
            AppointmentType type)
        {
            var appointments = await _unitOfWork.Appointments.FindAsync(a => a.AppointmentType == type);

            var dtos = _mapper.Map<IEnumerable<AppointmentDto>>(appointments);

            return OperationResult<IEnumerable<AppointmentDto>>.Success(dtos);
        }


        public async Task<OperationResult<int>> CreateAsync(CreateAppointmentDto dto)
        {
            if (dto.AppointmentDate <= DateTime.Now)
                return OperationResult<int>.Failure("Appointment date must be in the future.");

            var doctor = await _unitOfWork.Doctors.GetByIdAsync(dto.DoctorId);
            if (doctor is null || !doctor.IsAvailable)
                return OperationResult<int>.Failure("Doctor not found or unavailable.");

            var patient = await _unitOfWork.Patients.GetByIdAsync(dto.PatientId);
            if (patient is null)
                return OperationResult<int>.Failure("Patient not found.");

            var scheduleType = dto.AppointmentType == AppointmentType.Surgery
                ? ScheduleType.Surgery
                : ScheduleType.Consultation;

            var schedule = await _unitOfWork.DoctorSchedules.GetScheduleForSlotAsync(
                dto.DoctorId,
                dto.AppointmentDate,
                scheduleType);

            if (schedule is null || !schedule.IsActive)
                return OperationResult<int>.Failure(
                    "Selected slot is outside doctor's active schedule.");

          
            
            
            
            
            dto.DurationMinutes = schedule.SlotMinutes;
            dto.DoctorScheduleId = schedule.Id;

            var slotEnd = dto.AppointmentDate.AddMinutes(dto.DurationMinutes);
            var scheduleEnd = dto.AppointmentDate.Date + schedule.EndTime.ToTimeSpan();

            if (slotEnd > scheduleEnd)
                return OperationResult<int>.Failure(
                    "Selected appointment duration exceeds schedule end time.");

            var hasConflict = await _unitOfWork.Appointments.HasConflictAsync(
                dto.DoctorId,
                dto.AppointmentDate,
                dto.DurationMinutes);

            if (hasConflict)
                return OperationResult<int>.Failure(
                    "This time slot is no longer available.");

            var appointment = _mapper.Map<Appointment>(dto);

            appointment.Status = AppointmentStatus.Waiting;
            appointment.DoctorScheduleId = schedule.Id;
            appointment.DurationMinutes = schedule.SlotMinutes;

            await _unitOfWork.Appointments.AddAsync(appointment);
            await _unitOfWork.SaveChangesAsync();

            return OperationResult<int>.Success(
                appointment.Id,
                "Appointment booked successfully.");
        }


        public async Task<OperationResult> UpdateAsync(UpdateAppointmentDto dto)
        {
            var existing = await _unitOfWork.Appointments.GetByIdAsync(dto.Id);

            if (existing == null)
                return OperationResult.Failure("Appointment not found.");

            if (existing.Status == AppointmentStatus.Completed ||
                existing.Status == AppointmentStatus.Cancelled)
            {
                return OperationResult.Failure(
                    $"Cannot update an appointment with status '{existing.Status}'.");
            }

            if (dto.AppointmentDate <= DateTime.Now)
                return OperationResult.Failure("Appointment date must be in the future.");

            var scheduleChanged =
                existing.AppointmentDate != dto.AppointmentDate ||
                existing.DurationMinutes != dto.DurationMinutes;

            if (scheduleChanged)
            {
                var scheduleType = existing.AppointmentType == AppointmentType.Surgery
                    ? ScheduleType.Surgery
                    : ScheduleType.Consultation;

                var schedule = await _unitOfWork.DoctorSchedules.GetScheduleForSlotAsync(
                    existing.DoctorId,
                    dto.AppointmentDate,
                    scheduleType);

                if (schedule is null || !schedule.IsActive)
                    return OperationResult.Failure(
                        "Selected slot is outside doctor's active schedule.");

                var durationMinutes = schedule.SlotMinutes;

                var slotEnd = dto.AppointmentDate.AddMinutes(durationMinutes);
                var scheduleEnd = dto.AppointmentDate.Date + schedule.EndTime.ToTimeSpan();

                if (slotEnd > scheduleEnd)
                    return OperationResult.Failure(
                        "Selected appointment duration exceeds schedule end time.");

                var isAvailable = await IsTimeSlotAvailableAsync(
                    existing.DoctorId,
                    dto.AppointmentDate,
                    durationMinutes,
                    excludeAppointmentId: existing.Id);

                if (!isAvailable)
                    return OperationResult.Failure(
                        "The new time slot conflicts with another appointment.");

                existing.DoctorScheduleId = schedule.Id;
                existing.DurationMinutes = durationMinutes;
            }

            existing.AppointmentDate = dto.AppointmentDate;
            existing.Notes = dto.Notes;

            _unitOfWork.Appointments.Update(existing);
            await _unitOfWork.SaveChangesAsync();

            return OperationResult.Success("Appointment updated successfully.");
        }


        public async Task<OperationResult> DeleteAsync(int id)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);

            if (appointment == null)
                return OperationResult.Failure("Appointment not found.");

            if (appointment.Status == AppointmentStatus.InProgress ||
                appointment.Status == AppointmentStatus.Completed)
            {
                return OperationResult.Failure(
                    "Cannot delete an active or completed appointment. Use cancel instead.");
            }

            _unitOfWork.Appointments.Delete(appointment);
            await _unitOfWork.SaveChangesAsync();

            return OperationResult.Success("Appointment deleted.");
        }


        public async Task<OperationResult> StartAppointmentAsync(int id)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);

            if (appointment == null)
                return OperationResult.Failure("Appointment not found.");

            if (appointment.Status != AppointmentStatus.Waiting)
                return OperationResult.Failure(
                    "Only waiting appointments can be started.");

            appointment.Status = AppointmentStatus.InProgress;

            _unitOfWork.Appointments.Update(appointment);
            await _unitOfWork.SaveChangesAsync();

            return OperationResult.Success("Appointment started successfully.");
        }

        public async Task<OperationResult> CompleteAppointmentAsync(int id)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);

            if (appointment == null)
                return OperationResult.Failure("Appointment not found.");

            if (appointment.Status != AppointmentStatus.InProgress)
                return OperationResult.Failure(
                    "Only in-progress appointments can be completed.");

            appointment.Status = AppointmentStatus.Completed;

            _unitOfWork.Appointments.Update(appointment);
            await _unitOfWork.SaveChangesAsync();

            return OperationResult.Success("Appointment completed successfully.");
        }

        public async Task<OperationResult> CancelAppointmentAsync(
            int id,
            string? cancellationReason = null)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);

            if (appointment == null)
                return OperationResult.Failure("Appointment not found.");

            if (appointment.Status == AppointmentStatus.Completed)
                return OperationResult.Failure(
                    "Cannot cancel a completed appointment.");

            if (appointment.Status == AppointmentStatus.Cancelled)
                return OperationResult.Failure(
                    "Appointment is already cancelled.");

            appointment.Status = AppointmentStatus.Cancelled;
            appointment.CancellationReason = cancellationReason;

            _unitOfWork.Appointments.Update(appointment);
            await _unitOfWork.SaveChangesAsync();

            return OperationResult.Success("Appointment cancelled successfully.");
        }


        public async Task<bool> IsTimeSlotAvailableAsync(
            int doctorId,
            DateTime appointmentDate,
            int durationMinutes,
            int? excludeAppointmentId = null)
        {
            var existingAppointments =
                await _unitOfWork.Appointments.GetByDoctorAndDateAsync(
                    doctorId,
                    appointmentDate.Date);

            var newEnd = appointmentDate.AddMinutes(durationMinutes);

            foreach (var appointment in existingAppointments)
            {
                if (excludeAppointmentId.HasValue &&
                    appointment.Id == excludeAppointmentId.Value)
                {
                    continue;
                }

                if (appointment.Status == AppointmentStatus.Cancelled ||
                    appointment.Status == AppointmentStatus.Completed)
                {
                    continue;
                }

                var existingStart = appointment.AppointmentDate;
                var existingEnd = existingStart.AddMinutes(appointment.DurationMinutes);

                var overlaps =
                    existingStart < newEnd &&
                    existingEnd > appointmentDate;

                if (overlaps)
                    return false;
            }

            return true;
        }

        public async Task<bool> HasConflictingAppointmentsAsync(
            int doctorId,
            DateTime startDate,
            DateTime endDate)
        {
            var durationMinutes = (int)(endDate - startDate).TotalMinutes;

            return await _unitOfWork.Appointments.HasConflictAsync(
                doctorId,
                startDate,
                durationMinutes);
        }

        public async Task<int> GetTodayAppointmentsCountAsync()
        {
            return await _unitOfWork.Appointments.GetTodayCountAsync();
        }
    }
}