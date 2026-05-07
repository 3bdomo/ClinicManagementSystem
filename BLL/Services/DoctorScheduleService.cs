using AutoMapper;
using BLL.DTOs.Doctor;
using BLL.Interfaces;
using ClinicSystem.DAL.Models;
using Common.Enums;
using Common.Results;
using DAL.Interfaces;
using Microsoft.Extensions.Configuration;

namespace BLL.Services;

public class DoctorScheduleService : IDoctorScheduleService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;

    public DoctorScheduleService(IUnitOfWork uow, IMapper mapper, IConfiguration config)
    {
        _uow = uow;
        _mapper = mapper;
        _config = config;
    }

    public async Task<OperationResult<IEnumerable<DoctorScheduleDto>>> GetByDoctorAsync(int doctorId)
    {
        var schedules = await _uow.DoctorSchedules.GetByDoctorAsync(doctorId);
        return OperationResult<IEnumerable<DoctorScheduleDto>>.Success(
            _mapper.Map<IEnumerable<DoctorScheduleDto>>(schedules));
    }

    public async Task<OperationResult<IEnumerable<DoctorScheduleDto>>> GetByDoctorAndTypeAsync(int doctorId, ScheduleType type)
    {
        var schedules = await _uow.DoctorSchedules.GetByDoctorAsync(doctorId);
        schedules = schedules.Where(s => s.ScheduleType == type);
        return OperationResult<IEnumerable<DoctorScheduleDto>>.Success(
            _mapper.Map<IEnumerable<DoctorScheduleDto>>(schedules));
    }

    public async Task<OperationResult<DoctorScheduleDto>> GetByIdAsync(int id)
    {
        var schedule = await _uow.DoctorSchedules.GetByIdAsync(id);
        if (schedule is null)
            return OperationResult<DoctorScheduleDto>.Failure("Schedule not found");

        return OperationResult<DoctorScheduleDto>.Success(_mapper.Map<DoctorScheduleDto>(schedule));
    }

    public async Task<OperationResult<int>> CreateAsync(DoctorScheduleDto dto)
    {
        if (dto.StartTime >= dto.EndTime)
            return OperationResult<int>.Failure("Start time must be before end time");
        if (dto.SlotMinutes > 0 && dto.StartTime.AddMinutes(dto.SlotMinutes) != dto.EndTime)
            return OperationResult<int>.Failure("End time must equal start time + slot minutes");
        if (dto.DayOfWeek.HasValue == dto.SpecificDate.HasValue)
            return OperationResult<int>.Failure(
                "Specify either a recurring weekday or a specific date — not both");

        if (dto.SpecificDate.HasValue && dto.SpecificDate.Value < DateOnly.FromDateTime(DateTime.Today))
            return OperationResult<int>.Failure("Cannot create a schedule for a past date");

        bool hasConflict = await _uow.DoctorSchedules.HasTimeConflictAsync(
            dto.DoctorId,
            dto.DayOfWeek,
            dto.SpecificDate,
            dto.StartTime,
            dto.EndTime,
            dto.ScheduleType
        );

        if (hasConflict)
            return OperationResult<int>.Failure(
                $"Another {dto.ScheduleType} schedule for this doctor conflicts with the given time");

        if (dto.SlotMinutes <= 0)
        {
            var configKey = dto.ScheduleType == ScheduleType.Surgery
                ? "AppSettings:SurgerySlotMinutes"
                : "AppSettings:AppointmentSlotMinutes";

            var raw = _config[configKey];
            if (!int.TryParse(raw, out var slotMinutes))
            {
                slotMinutes = dto.ScheduleType == ScheduleType.Surgery ? 60 : 30;
            }

            dto.SlotMinutes = slotMinutes;
        }

        var entity = _mapper.Map<DoctorSchedule>(dto);
        await _uow.DoctorSchedules.AddAsync(entity);
        await _uow.SaveChangesAsync();

        return OperationResult<int>.Success(entity.Id, "Schedule created successfully");
    }

    public async Task<OperationResult> UpdateAsync(DoctorScheduleDto dto)
    {
        var existing = await _uow.DoctorSchedules.GetByIdAsync(dto.Id);
        if (existing is null)
            return OperationResult.Failure("Schedule not found");

        if (dto.StartTime >= dto.EndTime)
            return OperationResult.Failure("Start time must be before end time");
        if (dto.SlotMinutes > 0 && dto.StartTime.AddMinutes(dto.SlotMinutes) != dto.EndTime)
            return OperationResult.Failure("End time must equal start time + slot minutes");

        if (dto.DayOfWeek.HasValue == dto.SpecificDate.HasValue)
            return OperationResult.Failure("Specify either a weekday or a specific date — not both");

        bool hasConflict = await _uow.DoctorSchedules.HasTimeConflictAsync(
            dto.DoctorId,
            dto.DayOfWeek,
            dto.SpecificDate,
            dto.StartTime,
            dto.EndTime,
            dto.ScheduleType,
            excludeId: dto.Id
        );

        if (hasConflict)
            return OperationResult.Failure("Another schedule conflicts with the given time");

        _mapper.Map(dto, existing);
        _uow.DoctorSchedules.Update(existing);
        await _uow.SaveChangesAsync();

        return OperationResult.Success("Schedule updated successfully");
    }

    public async Task<OperationResult> ToggleActiveAsync(int id)
    {
        var schedule = await _uow.DoctorSchedules.GetByIdAsync(id);
        if (schedule is null)
            return OperationResult.Failure("Schedule not found");

        schedule.IsActive = !schedule.IsActive;
        _uow.DoctorSchedules.Update(schedule);
        await _uow.SaveChangesAsync();

        return OperationResult.Success(schedule.IsActive ? "Schedule activated" : "Schedule deactivated");
    }

    public async Task<OperationResult> DeleteAsync(int id)
    {
        var schedule = await _uow.DoctorSchedules.GetByIdAsync(id);
        if (schedule is null)
            return OperationResult.Failure("Schedule not found");

        var futureAppointments = schedule.Appointments?
            .Count(a => a.Status != AppointmentStatus.Cancelled
                        && a.AppointmentDate > DateTime.UtcNow);

        if (futureAppointments > 0)
            return OperationResult.Failure(
                $"Cannot delete schedule — there are {futureAppointments} upcoming appointment(s)");

        _uow.DoctorSchedules.Delete(schedule);
        await _uow.SaveChangesAsync();

        return OperationResult.Success("Schedule deleted successfully");
    }

    public async Task<OperationResult<IEnumerable<TimeSlotDto>>> GetAvailableSlotsAsync(
        int doctorId, DateTime date, ScheduleType type)
    {
       
        var doctor = await _uow.Doctors.GetByIdAsync(doctorId);
        if (doctor is null || !doctor.IsAvailable)
            return OperationResult<IEnumerable<TimeSlotDto>>.Failure("Doctor is not currently available");

        var schedule = await _uow.DoctorSchedules.GetScheduleForSlotAsync(doctorId, date, type);

        if (schedule is null || !schedule.IsActive)
            return OperationResult<IEnumerable<TimeSlotDto>>.Success(
                Enumerable.Empty<TimeSlotDto>());

        var slots = new List<TimeSlotDto>();
        var current = schedule.StartTime;

        while (current.AddMinutes(schedule.SlotMinutes) <= schedule.EndTime)
        {
            var slotStart = date.Date + current.ToTimeSpan();
            var slotEnd = slotStart.AddMinutes(schedule.SlotMinutes);

            slots.Add(new TimeSlotDto
            {
                DoctorId = doctorId,
                SlotStart = slotStart,
                SlotEnd = slotEnd,
                IsAvailable = true,
                ScheduleType = type,
                SlotMinutes = schedule.SlotMinutes
            });

            current = current.AddMinutes(schedule.SlotMinutes);
        }

        var booked = await _uow.Appointments.GetByDoctorAndDateAsync(doctorId, date.Date);
        booked = booked.Where(a => a.Status != AppointmentStatus.Cancelled).ToList();

        foreach (var slot in slots)
        {
            foreach (var appt in booked)
            {
                var bookedStart = appt.AppointmentDate;
                var bookedEnd = bookedStart.AddMinutes(appt.DurationMinutes);

                if (bookedStart < slot.SlotEnd && bookedEnd > slot.SlotStart)
                {
                    slot.IsAvailable = false;
                    break;
                }
            }
        }

        return OperationResult<IEnumerable<TimeSlotDto>>.Success(
            slots.Where(s => s.IsAvailable));
    }

    public async Task<OperationResult<DoctorScheduleDto?>> GetScheduleForSlotAsync(
        int doctorId, DateTime dateTime, ScheduleType type)
    {
        var entity = await _uow.DoctorSchedules.GetScheduleForSlotAsync(doctorId, dateTime, type);

        if (entity is null)
            return OperationResult<DoctorScheduleDto?>.Success(null);

        return OperationResult<DoctorScheduleDto?>.Success(_mapper.Map<DoctorScheduleDto>(entity));
    }
}