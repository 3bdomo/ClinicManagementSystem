using AutoMapper;
using BLL.DTOs.Doctor;
using BLL.Interfaces;
using ClinicSystem.DAL.Models;
using Common.Enums;
using Common.Results;
using DAL.Interfaces;
using Microsoft.Extensions.Configuration;

namespace BLL.Services;

public class DoctorScheduleService:IDoctorScheduleService
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
            return OperationResult<DoctorScheduleDto>.Failure("الجدول غير موجود");

        return OperationResult<DoctorScheduleDto>.Success(_mapper.Map<DoctorScheduleDto>(schedule));
    }

    public async Task<OperationResult<int>> CreateAsync(DoctorScheduleDto dto)
    {
        // 1. Time sanity check (also enforced via DB constraint, but validate here too)
        if (dto.StartTime >= dto.EndTime)
            return OperationResult<int>.Failure("وقت البداية يجب أن يكون قبل وقت النهاية");

        // 2. XOR validation: must have exactly one of DayOfWeek or SpecificDate
        if (dto.DayOfWeek.HasValue == dto.SpecificDate.HasValue) // both null or both set
            return OperationResult<int>.Failure(
                "حدد إما يوم أسبوعي متكرر أو تاريخ محدد — وليس الاثنين معاً");

        // 3. If one-time schedule: date must be in the future
        if (dto.SpecificDate.HasValue && dto.SpecificDate.Value < DateOnly.FromDateTime(DateTime.Today))
            return OperationResult<int>.Failure("لا يمكن إنشاء جدول لتاريخ في الماضي");

        // 4. Conflict check — same doctor, same schedule type, overlapping time
        bool hasConflict = await _uow.DoctorSchedules.HasTimeConflictAsync(
            dto.DoctorId,
            dto.DayOfWeek,
            dto.SpecificDate,
            dto.StartTime,
            dto.EndTime,
            dto.ScheduleType
            // excludeId: null (new record)
        );

        if (hasConflict)
            return OperationResult<int>.Failure(
                $"يوجد جدول {dto.ScheduleType} آخر للدكتور يتعارض مع هذا الوقت");

        // 5. Apply default SlotMinutes from config if not provided
        if (dto.SlotMinutes <= 0)
        {
            var configKey = dto.ScheduleType == ScheduleType.Surgery
                ? "ClinicSettings:SurgerySlotMinutes"
                : "ClinicSettings:AppointmentSlotMinutes";

            // IConfiguration.GetValue<> extension may not be available; read and parse safely
            var raw = _config[configKey];
            if (!int.TryParse(raw, out var slotMinutes))
            {
                // sensible defaults
                slotMinutes = dto.ScheduleType == ScheduleType.Surgery ? 60 : 30;
            }

            dto.SlotMinutes = slotMinutes;
        }

        var entity = _mapper.Map<DoctorSchedule>(dto);
        await _uow.DoctorSchedules.AddAsync(entity);
        await _uow.SaveChangesAsync();

        return OperationResult<int>.Success(entity.Id, "تم إضافة الجدول بنجاح");
    }

    public async Task<OperationResult> UpdateAsync(DoctorScheduleDto dto)
    {
        var existing = await _uow.DoctorSchedules.GetByIdAsync(dto.Id);
        if (existing is null)
            return OperationResult.Failure("الجدول غير موجود");

        if (dto.StartTime >= dto.EndTime)
            return OperationResult.Failure("وقت البداية يجب أن يكون قبل وقت النهاية");

        if (dto.DayOfWeek.HasValue == dto.SpecificDate.HasValue)
            return OperationResult.Failure("حدد إما يوم أسبوعي أو تاريخ محدد — وليس الاثنين معاً");

        bool hasConflict = await _uow.DoctorSchedules.HasTimeConflictAsync(
            dto.DoctorId,
            dto.DayOfWeek,
            dto.SpecificDate,
            dto.StartTime,
            dto.EndTime,
            dto.ScheduleType,
            excludeId: dto.Id   // ← exclude self
        );

        if (hasConflict)
            return OperationResult.Failure("يوجد جدول آخر يتعارض مع هذا الوقت");

        _mapper.Map(dto, existing); // update existing entity in-place
        _uow.DoctorSchedules.Update(existing);
        await _uow.SaveChangesAsync();

        return OperationResult.Success("تم تحديث الجدول");
    }

    public async Task<OperationResult> ToggleActiveAsync(int id)
    {
        var schedule = await _uow.DoctorSchedules.GetByIdAsync(id);
        if (schedule is null)
            return OperationResult.Failure("الجدول غير موجود");

        schedule.IsActive = !schedule.IsActive;
        _uow.DoctorSchedules.Update(schedule);
        await _uow.SaveChangesAsync();

        return OperationResult.Success(schedule.IsActive ? "تم تفعيل الجدول" : "تم تعطيل الجدول");
    }

    public async Task<OperationResult> DeleteAsync(int id)
    {
        var schedule = await _uow.DoctorSchedules.GetByIdAsync(id);
        if (schedule is null)
            return OperationResult.Failure("الجدول غير موجود");

        // DB has DeleteBehavior.Restrict on Appointments nav,
        // but we validate here first to give a user-friendly message
        var futureAppointments = schedule.Appointments?
            .Count(a => a.Status != AppointmentStatus.Cancelled
                        && a.AppointmentDate > DateTime.UtcNow);

        if (futureAppointments > 0)
            return OperationResult.Failure($"لا يمكن حذف الجدول يوجد موعد مستقبلي {futureAppointments}");

        _uow.DoctorSchedules.Delete(schedule);
        await _uow.SaveChangesAsync();

        return OperationResult.Success("تم حذف الجدول");
    }

    public async Task<OperationResult<IEnumerable<TimeSlotDto>>> GetAvailableSlotsAsync(
    int doctorId, DateTime date, ScheduleType type)
{
    // 1. Check Doctor.IsAvailable
    var doctor = await _uow.Doctors.GetByIdAsync(doctorId);
    if (doctor is null || !doctor.IsAvailable)
        return OperationResult<IEnumerable<TimeSlotDto>>.Failure("الدكتور غير متاح حالياً");

    // 2. Find active schedule for this doctor/date/type
    var dateOnly = date.Date;
    var schedule = await _uow.DoctorSchedules.GetScheduleForSlotAsync(doctorId, date, type);

    if (schedule is null || !schedule.IsActive)
        return OperationResult<IEnumerable<TimeSlotDto>>.Success(
            Enumerable.Empty<TimeSlotDto>());

    // 3. Generate all raw slots
    var slots = new List<TimeSlotDto>();
    var current = schedule.StartTime;

    while (current.AddMinutes(schedule.SlotMinutes) <= schedule.EndTime)
    {
        var slotStart = date.Date + current.ToTimeSpan();
        var slotEnd   = slotStart.AddMinutes(schedule.SlotMinutes);

        slots.Add(new TimeSlotDto
        {
            DoctorId     = doctorId,
            SlotStart    = slotStart,
            SlotEnd      = slotEnd,
            IsAvailable  = true,   // will be updated below
            ScheduleType = type,
            SlotMinutes  = schedule.SlotMinutes
        });

        current = current.AddMinutes(schedule.SlotMinutes);
    }

    // 4. Get booked appointments for this doctor on this date
    var booked = await _uow.Appointments.GetByDoctorAndDateAsync(doctorId, dateOnly);
    booked = booked.Where(a => a.Status != AppointmentStatus.Cancelled).ToList();

    // 5. Mark overlapping slots as unavailable
    foreach (var slot in slots)
    {
        foreach (var appt in booked)
        {
            var bookedStart = appt.AppointmentDate;
            var bookedEnd   = bookedStart.AddMinutes(appt.DurationMinutes);

            // overlap: bookedStart < slotEnd && bookedEnd > slotStart
            if (bookedStart < slot.SlotEnd && bookedEnd > slot.SlotStart)
            {
                slot.IsAvailable = false;
                break;
            }
        }
    }

    // 6. Return only available slots
    return OperationResult<IEnumerable<TimeSlotDto>>.Success(
        slots.Where(s => s.IsAvailable));
}

    public async Task<OperationResult<DoctorScheduleDto?>> GetScheduleForSlotAsync(
        int doctorId, DateTime dateTime, ScheduleType type)
    {
        // Repository handles the lookup logic:
        //   - SpecificDate match OR DayOfWeek match
        //   - IsActive == true
        //   - ScheduleType matches
        //   - dateTime.TimeOfDay is within StartTime-EndTime
        var entity = await _uow.DoctorSchedules.GetScheduleForSlotAsync(doctorId, dateTime, type);

        if (entity is null)
            return OperationResult<DoctorScheduleDto?>.Success(null); // no schedule found — caller decides

        return OperationResult<DoctorScheduleDto?>.Success(_mapper.Map<DoctorScheduleDto>(entity));
    }
}