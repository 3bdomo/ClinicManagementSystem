using BLL.DTOs.Doctor;
using BLL.DTOs.Patient;
using BLL.Interfaces;
using DAL.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.ViewModel;
using Web.ViewModel.Doctor;

[Authorize]
public class DoctorController : Controller
{
    private readonly IDoctorService _doctorService;
    private readonly IDoctorScheduleService _scheduleService;
    private readonly ClinicDbContext _context;

    public DoctorController(
        IDoctorService doctorService,
        IDoctorScheduleService scheduleService,
        ClinicDbContext context)
    {
        _doctorService = doctorService;
        _scheduleService = scheduleService;
        _context = context;
    }

    // ─── Index ────────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Index(string? searchQuery, int page = 1)
    {
        const int pageSize = 20;

        var result = await _doctorService.GetAllAsync(page, pageSize);

        List<DoctorFormViewModel> doctors = new();

        if (result.IsSuccess && result.Data is not null)
        {
            doctors = result.Data
                .Where(d =>
                    string.IsNullOrWhiteSpace(searchQuery) ||

                    (!string.IsNullOrWhiteSpace(d.FullName) &&
                     d.FullName.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)) ||

                    d.Specialization.ToString()
                        .Contains(searchQuery ?? "", StringComparison.OrdinalIgnoreCase)
                )
                .Select(MapToForm)
                .ToList();
        }

        var vm = new DoctorListViewModel
        {
            Doctors = doctors,
            SearchQuery = searchQuery ?? ""
        };

        return View(vm);
    }


    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> TopDoctorsByPatients()
    {
        var appointmentLinks = await _context.Appointments
            .Select(a => new
            {
                a.DoctorId,
                a.PatientId
            })
            .ToListAsync();

        var medicalRecordLinks = await _context.MedicalRecords
            .Select(m => new
            {
                m.DoctorId,
                m.PatientId
            })
            .ToListAsync();

        var topStats = appointmentLinks
            .Concat(medicalRecordLinks)
            .Distinct()
            .GroupBy(x => x.DoctorId)
            .Select(g => new
            {
                DoctorId = g.Key,
                PatientsCount = g.Count()
            })
            .OrderByDescending(x => x.PatientsCount)
            .Take(10)
            .ToList();

        var doctorIds = topStats.Select(x => x.DoctorId).ToList();

        var doctors = await _context.Doctors
            .Where(d => doctorIds.Contains(d.Id))
            .ToListAsync();

        var model = topStats
            .Join(
                doctors,
                stat => stat.DoctorId,
                doctor => doctor.Id,
                (stat, doctor) => new TopDoctorPatientsViewModel
                {
                    DoctorId = doctor.Id,
                    DoctorName = doctor.FullName ?? string.Empty,
                    Specialization = doctor.Specialization.ToString(),
                    Phone = doctor.Phone,
                    ConsultationFee = doctor.ConsultationFee,
                    PatientsCount = stat.PatientsCount
                })
            .ToList();

        return View("~/Views/Doctor/TopDoctorsByPatients.cshtml", model);
    }

    // ─── Details ──────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
            return BadRequest();

        var doctorResult = await _doctorService.GetByIdAsync(id.Value);

        if (!doctorResult.IsSuccess || doctorResult.Data is null)
            return NotFound();

        var schedulesResult = await _scheduleService.GetByDoctorAsync(id.Value);

        List<DoctorScheduleFormViewModel> schedules = new();

        if (schedulesResult.IsSuccess && schedulesResult.Data is not null)
        {
            schedules = schedulesResult.Data
                .Select(s => new DoctorScheduleFormViewModel
                {
                    Id = s.Id,
                    DoctorId = s.DoctorId,
                    DoctorName = s.DoctorName,
                    ScheduleType = s.ScheduleType,
                    DayOfWeek = s.DayOfWeek,
                    SpecificDate = s.SpecificDate,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    SlotMinutes = s.SlotMinutes,
                    IsActive = s.IsActive,
                    Notes = s.Notes
                })
                .ToList();
        }

        var vm = new DoctorDetailsViewModel
        {
            Doctor = MapToForm(doctorResult.Data),
            Schedules = schedules,
            TodayAppointments = new List<AppointmentRowViewModel>(),
            TotalAppointmentsCount = 0
        };

        return View(vm);
    }

    // ─── Edit GET ─────────────────────────────────────────────────────────────

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
            return BadRequest();

        var result = await _doctorService.GetByIdAsync(id.Value);

        if (!result.IsSuccess || result.Data is null)
            return NotFound();

        return View(MapToForm(result.Data));
    }

    // ─── Edit POST ────────────────────────────────────────────────────────────

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, DoctorFormViewModel vm)
    {
        if (id != vm.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(vm);

        var existing = await _doctorService.GetByIdAsync(vm.Id);

        if (!existing.IsSuccess || existing.Data is null)
            return NotFound();

        var dto = new DoctorDto
        {
            Id = vm.Id,
            FullName = vm.FullName,
            Specialization = vm.Specialization,
            ConsultationFee = vm.ConsultationFee,
            Bio = vm.Bio,
            IsAvailable = vm.IsAvailable,
            ApplicationUserId = existing.Data.ApplicationUserId
        };

        var result = await _doctorService.UpdateAsync(dto);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message ?? "Failed to update doctor.");
            return View(vm);
        }

        TempData["Success"] = $"Doctor \"{vm.FullName}\" updated successfully.";

        return RedirectToAction(nameof(Details), new { id = vm.Id });
    }

    // ─── Mapping Helper ──────────────────────────────────────────────────────

    private static DoctorFormViewModel MapToForm(DoctorDto dto)
    {
        return new DoctorFormViewModel
        {
            Id = dto.Id,
            FullName = dto.FullName ?? "",
            Specialization = dto.Specialization,
            ConsultationFee = dto.ConsultationFee ?? 0,
            Bio = dto.Bio ?? "",
            IsAvailable = dto.IsAvailable ?? true
        };
    }
}