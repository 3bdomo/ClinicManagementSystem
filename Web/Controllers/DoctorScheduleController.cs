using AutoMapper;
using BLL.DTOs.Doctor;
using BLL.Interfaces;
using Common.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Web.ViewModel;

public class DoctorScheduleController : Controller
{
    private const int DoctorsPageSize = 500;
    private readonly IDoctorScheduleService _doctorScheduleService;
    private readonly IDoctorService _doctorService;
    private readonly IMapper _mapper;

    public DoctorScheduleController(
        IDoctorScheduleService doctorScheduleService,
        IDoctorService doctorService,
        IMapper mapper)
    {
        _doctorScheduleService = doctorScheduleService;
        _doctorService = doctorService;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index(int? doctorId, ScheduleType? scheduleType)
    {
        var schedules = new List<DoctorScheduleDto>();

        if (doctorId.HasValue)
        {
            var result = scheduleType.HasValue
                ? await _doctorScheduleService.GetByDoctorAndTypeAsync(doctorId.Value, scheduleType.Value)
                : await _doctorScheduleService.GetByDoctorAsync(doctorId.Value);

            if (result.IsSuccess && result.Data != null)
            {
                schedules.AddRange(result.Data);
            }
            else if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Message;
            }
        }
        else
        {
            var doctorsResult = await _doctorService.GetAllAsync(1, DoctorsPageSize);
            if (doctorsResult.IsSuccess)
            {
                foreach (var doctor in doctorsResult.Data)
                {
                    var result = scheduleType.HasValue
                        ? await _doctorScheduleService.GetByDoctorAndTypeAsync(doctor.Id, scheduleType.Value)
                        : await _doctorScheduleService.GetByDoctorAsync(doctor.Id);

                    if (result.IsSuccess && result.Data != null)
                    {
                        schedules.AddRange(result.Data);
                    }
                }
            }
            else
            {
                TempData["ErrorMessage"] = doctorsResult.Message;
            }
        }

        var viewModel = new DoctorScheduleListViewModel
        {
            DoctorId = doctorId,
            ScheduleType = scheduleType,
            Schedules = _mapper.Map<IEnumerable<DoctorScheduleFormViewModel>>(schedules),
            Doctors = await BuildDoctorsSelectListAsync(doctorId)
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Create()
    {
        var viewModel = new DoctorScheduleFormViewModel
        {
            Doctors = await BuildDoctorsSelectListAsync(null)
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DoctorScheduleFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Doctors = await BuildDoctorsSelectListAsync(model.DoctorId);
            return View(model);
        }

        var dto = _mapper.Map<DoctorScheduleDto>(model);
        var result = await _doctorScheduleService.CreateAsync(dto);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message ?? "Failed to create schedule.");
            model.Doctors = await BuildDoctorsSelectListAsync(model.DoctorId);
            return View(model);
        }

        TempData["SuccessMessage"] = result.Message;
        return RedirectToAction(nameof(Index), new { doctorId = model.DoctorId });
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
        {
            return BadRequest();
        }

        var result = await _doctorScheduleService.GetByIdAsync(id.Value);
        if (!result.IsSuccess)
        {
            return NotFound();
        }

        var viewModel = _mapper.Map<DoctorScheduleFormViewModel>(result.Data);
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, DoctorScheduleFormViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var dto = _mapper.Map<DoctorScheduleDto>(model);
        var result = await _doctorScheduleService.UpdateAsync(dto);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message ?? "Failed to update schedule.");
            return View(model);
        }

        TempData["SuccessMessage"] = result.Message;
        return RedirectToAction(nameof(Index), new { doctorId = model.DoctorId });
    }

    private async Task<IEnumerable<SelectListItem>> BuildDoctorsSelectListAsync(int? selectedDoctorId)
    {
        var doctorsResult = await _doctorService.GetAllAsync(1, DoctorsPageSize);
        if (!doctorsResult.IsSuccess || doctorsResult.Data == null)
        {
            return [];
        }

        return doctorsResult.Data.Select(d => new SelectListItem
        {
            Value = d.Id.ToString(),
            Text = d.FullName,
            Selected = selectedDoctorId.HasValue && selectedDoctorId.Value == d.Id
        });
    }
}
