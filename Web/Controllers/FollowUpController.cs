using System;
using System.Linq;
using System.Collections.Generic;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.ViewModel;

[Authorize]
public class FollowUpController : Controller
{
    private readonly IMedicalRecordService _medicalRecordService;

    public FollowUpController(IMedicalRecordService medicalRecordService)
    {
        _medicalRecordService = medicalRecordService;
    }

    

    [HttpGet]
    public async Task<IActionResult> Index(DateTime? dateFrom, DateTime? dateTo)
    {
        var from = dateFrom ?? DateTime.Today;
        var to = dateTo ?? DateTime.Today.AddDays(30);

        if (from > to)
        {
            (from, to) = (to, from);
        }

        List<MedicalRecordRowViewModel> records = new();

        var result = await _medicalRecordService.GetUpcomingFollowUpsAsync(from, to);

        if (result.IsSuccess && result.Data is not null)
        {
            records = result.Data
                .Select(r => new MedicalRecordRowViewModel
                {
                    Id = r.Id,
                    PatientId = r.PatientId,
                    DoctorId = r.DoctorId,
                    AppointmentId = r.AppointmentId,
                    PatientName = r.PatientName ?? string.Empty,
                    DoctorName = r.DoctorName ?? string.Empty,
                    VisitDate = r.VisitedDate,
                    Diagnosis = r.Diagnosis ?? string.Empty,
                    FollowUpDate = r.FollowUpDate,
                    Notes = r.Notes ?? string.Empty
                })
                .OrderBy(r => r.FollowUpDate)
                .ToList();
        }
        else
        {
            TempData["Error"] = result.Message ?? "Failed to load follow-ups.";
        }

        var vm = new FollowUpListViewModel
        {
            MedicalRecords = records,
            DateFrom = from,
            DateTo = to,
            TotalCount = records.Count
        };

        return View(vm);
    }
}