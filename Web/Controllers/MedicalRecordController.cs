using BLL.DTOs.MedicalRecord;
using BLL.Interfaces;
using ClinicSystem.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Common.Enums;
using Web.ViewModel;
using System.Linq;
using BLL.DTOs.Patient;
using BLL.DTOs.Procedure;

public class MedicalRecordController : Controller
{
    private readonly IMedicalRecordService _medicalRecordService;
    private readonly IAppointmentService _appointmentService;
    private readonly IPatientService _patientService;

    public MedicalRecordController(IMedicalRecordService medicalRecordService, IAppointmentService appointmentService, IPatientService patientService)
    {
        _medicalRecordService = medicalRecordService;
        _appointmentService = appointmentService;
        _patientService = patientService;
    }

    
    public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
    {
        var result = await _medicalRecordService.GetAllAsync(pageNumber, pageSize);
        
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Message;
            return View(Enumerable.Empty<MedicalRecordRowViewModel>());
        }
        var vm = result.Data.Select(r => new MedicalRecordRowViewModel
        {
            Id = r.Id,
            PatientId = r.PatientId,
            DoctorId = r.DoctorId,
            AppointmentId = r.AppointmentId,
            PatientName = r.PatientName ?? "—",
            DoctorName = r.DoctorName ?? "—",
            VisitDate = r.VisitedDate,
            Diagnosis = r.Diagnosis ?? "—",
            FollowUpDate = r.FollowUpDate,
            Notes = r.Notes,
            ProceduresCount = r.ProceduresCount,
            AttachmentsCount = r.AttachmentsCount
        });
        ViewBag.PageNumber = pageNumber;
        ViewBag.PageSize = pageSize;
        
        return View(vm);
    }
    
    
    public async Task<IActionResult> Details(int? id)
    {
        if (id is null) return BadRequest();

        var result = await _medicalRecordService.GetFullAsync(id.Value);

        if (!result.IsSuccess || result.Data == null)
        {
            TempData["Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        var r = result.Data;

        var vm = new MedicalRecordDetailsViewModel
        {
            Record = new MedicalRecordRowViewModel
            {
                Id = r.Id,
                PatientId = r.PatientId,
                PatientName = r.PatientName,
                DoctorName = r.DoctorName,
                VisitDate = r.VisitedDate,
                FollowUpDate = r.FollowUpDate,
                Diagnosis = r.Diagnosis,
                Notes = r.Notes
            },

            
            CanEdit = true
        };

        return View(vm);
    }
    
    public async Task<IActionResult> Create(int? patientId)
    {
        var vm = new MedicalRecordFormViewModel
        {
            PatientId = patientId,
            Appointments = await GetPatientAppointmentsAsync(patientId)
        };

        var patientsList = new List<PatientDto>();
        var activeRes = await _patientService.GetAllAsync();
        if (activeRes.IsSuccess && activeRes.Data != null)
            patientsList.AddRange(activeRes.Data);

        var deletedRes = await _patientService.GetDeletedAsync();
        if (deletedRes.IsSuccess && deletedRes.Data != null)
            patientsList.AddRange(deletedRes.Data);

        ViewBag.Patients = patientsList.OrderBy(p => p.FullName).ToList();

        return View(vm);
    }
    
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
            FollowUpDate = vm.FollowUpDate ?? DateTime.Now.AddDays(7) 
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
                    .Where(a => !a.HasMedicalRecord && 
                                (a.Status == AppointmentStatus.Completed || a.Status == AppointmentStatus.InProgress))
                    .Select(a => new SelectListItem
                    {
                        Text = $"{a.AppointmentDate:yyyy-MM-dd HH:mm} - {a.DoctorName}",
                        Value = a.Id.ToString()
                    }).ToList();
            }
        }
        return appointments;
    }
    
    
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
    
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _medicalRecordService.DeleteAsync(id);
        TempData[ result.IsSuccess ?"Success" : "Error"]= result.Message;
        return RedirectToAction(nameof(Index));
    }
    
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
        return View("FollowUps", result.Data);
    }
    
    
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
    
    public async Task<IActionResult> GetByPatient(int patientId)
    {
        var result = await _medicalRecordService.GetByPatientAsync(patientId);
        if (!result.IsSuccess)
        {
            return Json(new { success = false, message = result.Message });
        }
        return Json(new { success = true, data = result.Data });
    }

    public async Task<IActionResult> GetAppointmentsByPatient(int patientId)
    {
        var history = await _appointmentService.GetPatientHistoryAsync(patientId);
        if (!history.IsSuccess)
            return Json(new { success = false, message = history.Message });

        var items = history.Data
            .Where(a => !a.HasMedicalRecord && 
                        (a.Status == AppointmentStatus.Completed || a.Status == AppointmentStatus.InProgress))
            .Select(a => new
            {
                id = a.Id,
                label = $"{a.AppointmentDate:yyyy-MM-dd HH:mm} - {a.DoctorName}",
                doctorId = a.DoctorId,
                doctorName = a.DoctorName
            }).ToList();

        return Json(new { success = true, data = items });
    }
    
}