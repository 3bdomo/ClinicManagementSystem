using BLL.DTOs.Appointment;
using BLL.DTOs.Statistics;
using BLL.Interfaces;
using ClinicSystem.DAL.Models;
using Common.Enums;
using Common.Results;
using DAL.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services;

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _unitOfWork;

    public DashboardService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<OperationResult<DashboardStatsDto>> GetStatsAsync()
    {
        try
        {
            var patients = await _unitOfWork.Patients.FindAsync(x => true);
            var patientsCount = patients.Count();
            
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            
            var todayAppointments = await _unitOfWork.Appointments.GetTodayAsync();
            var todayAppointmentsCount = todayAppointments.Count();

            var paidInvoices = await _unitOfWork.Invoices.FindAsync(i => i.CreatedAt >= today && i.CreatedAt < tomorrow && i.Status == InvoiceStatus.Paid);
            var todayRevenue = paidInvoices.Sum(i => i.TotalAmount);

            var pendingInvoices = await _unitOfWork.Invoices.FindAsync(i => i.Status == InvoiceStatus.Unpaid);
            var pendingInvoicesCount = pendingInvoices.Count();

            var upcomingFollowUps = await _unitOfWork.MedicalRecords.FindAsync(r => r.FollowUpDate != null && r.FollowUpDate.Value >= today);
            var upcomingFollowUpsCount = upcomingFollowUps.Count();

            var stats = new DashboardStatsDto
            {
                TotalPatients = patientsCount,
                TodayAppointments = todayAppointmentsCount,
                TodayRevenue = todayRevenue,
                PendingInvoicesCount = pendingInvoicesCount,
                UpcomingFollowUps = upcomingFollowUpsCount
            };

            return OperationResult<DashboardStatsDto>.Success(stats);
        }
        catch (Exception ex)
        {
            return OperationResult<DashboardStatsDto>.Failure($"Failed to get dashboard stats: {ex.Message}");
        }
    }

    public async Task<OperationResult<TodaySummaryDto>> GetTodaySummaryAsync()
    {
        try
        {
            var todayAppointments = await _unitOfWork.Appointments.GetTodayAsync();

            var dtos = todayAppointments.Select(a => new AppointmentDto
            {
                Id = a.Id,
                PatientId = a.PatientId,
                PatientName = a.Patient?.FullName ?? "Unknown",
                DoctorId = a.DoctorId,
                DoctorName = a.Doctor?.FullName ?? "Unknown",
                AppointmentDate = a.AppointmentDate,
                DurationMinutes = a.DurationMinutes,
                Status = a.Status
            }).Take(10).ToList();

            var summary = new TodaySummaryDto
            {
                Appointments = dtos
            };

            return OperationResult<TodaySummaryDto>.Success(summary);
        }
        catch (Exception ex)
        {
            return OperationResult<TodaySummaryDto>.Failure($"Failed to get today summary: {ex.Message}");
        }
    }
}
