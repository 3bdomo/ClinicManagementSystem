using AutoMapper;
using BLL.DTOs.Appointment;
using BLL.Interfaces;
using ClinicSystem.DAL.Models;
using Common.Enums;
using Common.Results;
using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            var appointments = await _unitOfWork.Appointments.GetAllAsync(1, int.MaxValue);
            var mapped = _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
            return OperationResult<IEnumerable<AppointmentDto>>.Success(mapped);
        }

        public async Task<OperationResult<IEnumerable<AppointmentDto>>> GetAllAsync(int pageNumber, int pageSize)
        {
            var appointments = await _unitOfWork.Appointments.GetAllAsync(pageNumber, pageSize);
            var mapped = _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
            return OperationResult<IEnumerable<AppointmentDto>>.Success(mapped);
        }

        public async Task<OperationResult<AppointmentDto>> GetByIdAsync(int id)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);
            if (appointment == null)
                return OperationResult<AppointmentDto>.Failure("Appointment not found");

            var mapped = _mapper.Map<AppointmentDto>(appointment);
            return OperationResult<AppointmentDto>.Success(mapped);
        }

        public async Task<OperationResult<int>> CreateAsync(AppointmentDto appointmentDto)
        {
            // --- FIX: Validate appointment date is in the future ---
            if (appointmentDto.AppointmentDate <= DateTime.Now)
                return OperationResult<int>.Failure("Appointment date must be in the future.");

            var isConflict = await _unitOfWork.Appointments.HasConflictAsync(
                appointmentDto.DoctorId,
                appointmentDto.AppointmentDate,
                appointmentDto.DurationMinutes);

            if (isConflict)
                return OperationResult<int>.Failure("There is a conflict with another appointment.");

            var appointment = _mapper.Map<Appointment>(appointmentDto);
            appointment.Status = AppointmentStatus.Waiting;

            await _unitOfWork.Appointments.AddAsync(appointment);
            await _unitOfWork.SaveChangesAsync();

            return OperationResult<int>.Success(appointment.Id);
        }

        public async Task<OperationResult> UpdateAsync(AppointmentDto appointmentDto)
        {
            var existing = await _unitOfWork.Appointments.GetByIdAsync(appointmentDto.Id);
            if (existing == null)
                return OperationResult.Failure("Appointment not found");

            if (existing.Status == AppointmentStatus.Completed || existing.Status == AppointmentStatus.Cancelled)
                return OperationResult.Failure($"Cannot update an appointment with status '{existing.Status}'.");

            bool dateOrDoctorChanged =
                existing.DoctorId != appointmentDto.DoctorId ||
                existing.AppointmentDate != appointmentDto.AppointmentDate ||
                existing.DurationMinutes != appointmentDto.DurationMinutes;

            if (dateOrDoctorChanged)
            {
                var isAvailable = await IsTimeSlotAvailableAsync(
                    appointmentDto.DoctorId,
                    appointmentDto.AppointmentDate,
                    appointmentDto.DurationMinutes,
                    excludeAppointmentId: existing.Id);

                if (!isAvailable)
                    return OperationResult.Failure("There is a scheduling conflict with another appointment.");
            }

            existing.DoctorId = appointmentDto.DoctorId;
            existing.PatientId = appointmentDto.PatientId;
            existing.AppointmentDate = appointmentDto.AppointmentDate;
            existing.DurationMinutes = appointmentDto.DurationMinutes;
            existing.Notes = appointmentDto.Notes;

            _unitOfWork.Appointments.Update(existing);
            await _unitOfWork.SaveChangesAsync();

            return OperationResult.Success();
        }

        public async Task<OperationResult> DeleteAsync(int id)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);
            if (appointment == null)
                return OperationResult.Failure("Appointment not found");

            if (appointment.Status == AppointmentStatus.InProgress)
                return OperationResult.Failure("Cannot delete an appointment that is currently in progress.");

            _unitOfWork.Appointments.Delete(appointment);
            await _unitOfWork.SaveChangesAsync();

            return OperationResult.Success();
        }

        public async Task<OperationResult<IEnumerable<AppointmentHistoryDto>>> GetPatientHistoryAsync(int patientId)
        {
            var appointments = await _unitOfWork.Appointments.GetByPatientAsync(patientId);
            var mapped = _mapper.Map<IEnumerable<AppointmentHistoryDto>>(appointments);
            return OperationResult<IEnumerable<AppointmentHistoryDto>>.Success(mapped);
        }

        public async Task<OperationResult<IEnumerable<AppointmentHistoryDto>>> GetDoctorHistoryAsync(int doctorId, int pageNumber, int pageSize)
        {
            var appointments = await _unitOfWork.Appointments.GetByDoctorAsync(doctorId, pageNumber, pageSize);
            var mapped = _mapper.Map<IEnumerable<AppointmentHistoryDto>>(appointments);
            return OperationResult<IEnumerable<AppointmentHistoryDto>>.Success(mapped);
        }

        public async Task<OperationResult<IEnumerable<AppointmentDto>>> GetDoctorAppointmentsAsync(int doctorId)
        {
            var appointments = await _unitOfWork.Appointments.GetByDoctorAsync(doctorId);
            var mapped = _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
            return OperationResult<IEnumerable<AppointmentDto>>.Success(mapped);
        }

        public async Task<OperationResult<IEnumerable<AppointmentDto>>> GetDoctorAppointmentsByDateAsync(int doctorId, DateTime date)
        {
            var appointments = await _unitOfWork.Appointments.GetByDoctorAndDateAsync(doctorId, date);
            var mapped = _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
            return OperationResult<IEnumerable<AppointmentDto>>.Success(mapped);
        }

        public async Task<OperationResult<IEnumerable<AppointmentDto>>> GetAppointmentsByStatusAsync(AppointmentStatus status)
        {
            var appointments = await _unitOfWork.Appointments.FindAsync(a => a.Status == status);
            var mapped = _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
            return OperationResult<IEnumerable<AppointmentDto>>.Success(mapped);
        }

        public async Task<OperationResult<IEnumerable<AppointmentDto>>> GetAppointmentsByTypeAsync(AppointmentType type)
        {
            var appointments = await _unitOfWork.Appointments.FindAsync(a => a.AppointmentType == type);
            var mapped = _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
            return OperationResult<IEnumerable<AppointmentDto>>.Success(mapped);
        }

        public async Task<OperationResult> StartAppointmentAsync(int id)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);
            if (appointment == null)
                return OperationResult.Failure("Appointment not found");

            if (appointment.Status != AppointmentStatus.Waiting)
                return OperationResult.Failure($"Cannot start an appointment with status '{appointment.Status}'. Only 'Waiting' appointments can be started.");

            appointment.Status = AppointmentStatus.InProgress;
            _unitOfWork.Appointments.Update(appointment);
            await _unitOfWork.SaveChangesAsync();

            return OperationResult.Success();
        }

        public async Task<OperationResult> CompleteAppointmentAsync(int id)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);
            if (appointment == null)
                return OperationResult.Failure("Appointment not found");

            if (appointment.Status != AppointmentStatus.InProgress)
                return OperationResult.Failure($"Cannot complete an appointment with status '{appointment.Status}'. Only 'InProgress' appointments can be completed.");

            appointment.Status = AppointmentStatus.Completed;
            _unitOfWork.Appointments.Update(appointment);
            await _unitOfWork.SaveChangesAsync();

            return OperationResult.Success();
        }

        public async Task<OperationResult> CancelAppointmentAsync(int id, string? cancellationReason = null)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);
            if (appointment == null)
                return OperationResult.Failure("Appointment not found");

            if (appointment.Status == AppointmentStatus.Completed)
                return OperationResult.Failure("Cannot cancel an appointment that has already been completed.");

            if (appointment.Status == AppointmentStatus.Cancelled)
                return OperationResult.Failure("Appointment is already cancelled.");

            appointment.Status = AppointmentStatus.Cancelled;

            if (!string.IsNullOrWhiteSpace(cancellationReason))
            {
                appointment.Notes = string.IsNullOrEmpty(appointment.Notes)
                    ? $"Cancellation Reason: {cancellationReason}"
                    : $"{appointment.Notes}\nCancellation Reason: {cancellationReason}";
            }

            _unitOfWork.Appointments.Update(appointment);
            await _unitOfWork.SaveChangesAsync();

            return OperationResult.Success();
        }

        public async Task<bool> IsTimeSlotAvailableAsync(int doctorId, DateTime appointmentDate, int durationMinutes, int? excludeAppointmentId = null)
        {
            var apps = await _unitOfWork.Appointments.GetByDoctorAndDateAsync(doctorId, appointmentDate.Date);
            var endTime = appointmentDate.AddMinutes(durationMinutes);

            foreach (var a in apps)
            {
                if (excludeAppointmentId.HasValue && a.Id == excludeAppointmentId.Value)
                    continue;

                if (a.Status == AppointmentStatus.Cancelled || a.Status == AppointmentStatus.Completed)
                    continue;

                var aEnd = a.AppointmentDate.AddMinutes(a.DurationMinutes);
                if (appointmentDate < aEnd && endTime > a.AppointmentDate)
                    return false;
            }

            return true;
        }

        public async Task<OperationResult<IEnumerable<DateTime>>> GetAvailableSlotsAsync(int doctorId, DateTime date, int slotDurationMinutes = 30)
        {
            var slots = new List<DateTime>();
            var startTime = date.Date.AddHours(9);
            var endTime = date.Date.AddHours(17);

            var existingAppointments = await _unitOfWork.Appointments.GetByDoctorAndDateAsync(doctorId, date);

            var activeAppointments = existingAppointments
                .Where(a => a.Status != AppointmentStatus.Cancelled && a.Status != AppointmentStatus.Completed)
                .ToList();

            while (startTime.AddMinutes(slotDurationMinutes) <= endTime)
            {
                var slotEnd = startTime.AddMinutes(slotDurationMinutes);
                var isAvailable = !activeAppointments.Any(a =>
                    startTime < a.AppointmentDate.AddMinutes(a.DurationMinutes) &&
                    slotEnd > a.AppointmentDate);

                if (isAvailable)
                    slots.Add(startTime);

                startTime = startTime.AddMinutes(slotDurationMinutes);
            }

            return OperationResult<IEnumerable<DateTime>>.Success(slots);
        }

        public Task<OperationResult<IEnumerable<AppointmentDto>>> GetDeletedAsync()
        {
            return Task.FromResult(
                OperationResult<IEnumerable<AppointmentDto>>.Failure("Soft-delete is not supported for Appointments."));
        }

        public Task<OperationResult> RestoreAsync(int id)
        {
            return Task.FromResult(
                OperationResult.Failure("Restore functionality is not supported for Appointments."));
        }

        public async Task<int> GetTodayAppointmentsCountAsync()
        {
            var apps = await _unitOfWork.Appointments.GetTodayAsync();
            return apps.Count();
        }

        public async Task<bool> HasConflictingAppointmentsAsync(int doctorId, DateTime startDate, DateTime endDate)
        {
            var duration = (int)(endDate - startDate).TotalMinutes;
            return await _unitOfWork.Appointments.HasConflictAsync(doctorId, startDate, duration);
        }
    }
}