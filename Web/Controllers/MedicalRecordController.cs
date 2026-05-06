using BLL.DTOs.MedicalRecord;
using BLL.Interfaces;
using ClinicSystem.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Common.Enums;
using Web.ViewModel;
using System.Linq;

public class MedicalRecordController : Controller
{
    private readonly IMedicalRecordService _medicalRecordService;
    private readonly IAppointmentService _appointmentService;

    public MedicalRecordController(IMedicalRecordService medicalRecordService, IAppointmentService appointmentService)
    {
        _medicalRecordService = medicalRecordService;
        _appointmentService = appointmentService;
    }

    // GET /MedicalRecord/Index
    public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
    {
        var result = await _medicalRecordService.GetAllAsync(pageNumber, pageSize);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Message;
            return View(Enumerable.Empty<MedicalRecordDto>());
        }
        ViewBag.PageNumber = pageNumber;
        ViewBag.PageSize = pageSize;
        
        return View(result.Data);
    }
    
    // GET /MedicalRecord/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id is null) return BadRequest();
        var result = await _medicalRecordService.GetFullAsync(id.Value);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
        return View(result.Data);
    }
    
    // GET /MedicalRecord/Create?patientId=5
    public async Task<IActionResult> Create(int? patientId)
    {
        var vm = new MedicalRecordFormViewModel
        {
            PatientId = patientId,
            Appointments = await GetPatientAppointmentsAsync(patientId)
        };
        return View(vm);
    }
    //POST /MedicalRecord/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MedicalRecordFormViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            vm.Appointments = await GetPatientAppointmentsAsync(vm.PatientId);
            return View(vm);
        }

        var appointmentResult = await _appointmentService.GetByIdAsync(vm.AppointmentId);
        if (!appointmentResult.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, "Appointment not found.");
            vm.Appointments = await GetPatientAppointmentsAsync(vm.PatientId);
            return View(vm);
        }

        var appointment = appointmentResult.Data;

        var dto = new CreateMedicalRecordDto
        {
            AppointmentId = vm.AppointmentId,
            PatientId = appointment.PatientId,
            DoctorId = appointment.DoctorId,
            Diagnosis = vm.Diagnosis,
            Notes = vm.Notes,
            VisitedDate = appointment.AppointmentDate,
            FollowUpDate = vm.FollowUpDate ?? DateTime.Now.AddDays(7) // Default or required?
        };

        var result = await _medicalRecordService.CreateAsync(dto);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            vm.Appointments = await GetPatientAppointmentsAsync(vm.PatientId);
            return View(vm);
        }
        TempData["Success"] = "Medical record created successfully.";
        return RedirectToAction(nameof(Details), new { id = result.Data });
    }

    private async Task<List<SelectListItem>> GetPatientAppointmentsAsync(int? patientId)
    {
        var appointments = new List<SelectListItem>();
        if (patientId.HasValue)
        {
            var history = await _appointmentService.GetPatientHistoryAsync(patientId.Value);
            if (history.IsSuccess)
            {
                appointments = history.Data
                    .Where(a => !a.HasMedicalRecord && a.Status == AppointmentStatus.Completed)
                    .Select(a => new SelectListItem
                    {
                        Text = $"{a.AppointmentDate:yyyy-MM-dd HH:mm} - {a.DoctorName}",
                        Value = a.Id.ToString()
                    }).ToList();
            }
        }
        return appointments;
    }
    
    // GET /MedicalRecord/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null) return BadRequest();
        var result = await _medicalRecordService.GetByIdAsync(id.Value);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
        var dto = new UpdateMedicalRecordDto()
        {
            Id = result.Data.Id,
            Diagnosis = result.Data.Diagnosis,
            Notes = result.Data.Notes,
            FollowUpDate = result.Data.FollowUpDate
        };
        return View(dto);
    }
    
    // POST /MedicalRecord/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id,UpdateMedicalRecordDto dto)
    {
        if (id is null) return BadRequest();
        if(!ModelState.IsValid)return View(dto);
        var result = await _medicalRecordService.UpdateAsync(dto);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(dto);
        }

        TempData["Success"] = result.Message;
        return RedirectToAction(nameof(Details), new { id });
    }
    
    // POST /MedicalRecord/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _medicalRecordService.DeleteAsync(id);
        TempData[ result.IsSuccess ?"Success" : "Error"]= result.Message;
        return RedirectToAction(nameof(Index));
    }
    //Get/MedicalRecord/ByPatient/5
    public async Task<IActionResult> ByPatient(int patientId)
    {
        var result = await _medicalRecordService.GetByPatientAsync(patientId);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
        ViewBag.PatientId = patientId;
        return View(result.Data);
    }
    
    //Get/MedicalRecord/ByAppointment/5
    public async Task<IActionResult> ByAppointment(int appointmentId)
    {
        var result = await _medicalRecordService.GetByAppointmentAsync(appointmentId);
        if (!result.IsSuccess)    
        {
            TempData["Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
        return RedirectToAction(nameof(Details), new { id = result.Data.Id });
    }
    
    //Get/MedicalRecord/FollowUp/5
    public async Task<IActionResult> FollowUp(DateTime? from, DateTime? to)
    {
        var startDate = from ?? DateTime.Now;
        var endDate = to ?? DateTime.Now.AddDays(7);
        var result = await _medicalRecordService.GetUpcomingFollowUpsAsync(startDate, endDate);
        if(!result.IsSuccess)
        {
            TempData["Error"] = result.Message;
            return View(Enumerable.Empty<MedicalRecordDto>());
        }
        ViewBag.From = startDate;
        ViewBag.To = endDate;
        return View(result.Data);
    }
    
    //GET /MedicalRecord/PatientStatistics/5
    public async Task<IActionResult> PatientStatistics(int patientId)
    {
        var result = await _medicalRecordService.GetPatientStatisticsAsync(patientId);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
        return View(result.Data);
    }
    // GET /MedicalRecord/GetByPatient/5  (AJAX)
    public async Task<IActionResult> GetByPatient(int patientId)
    {
        var result = await _medicalRecordService.GetByPatientAsync(patientId);
        if (!result.IsSuccess)
        {
            return Json(new { success = false, message = result.Message });
        }
        return Json(new { success = true, data = result.Data });
    }
    
}
