using BLL.DTOs.Billing;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Common.Enums;
using AutoMapper;
using Web.ViewModel;
using System.Linq;

public class BillingController : Controller
{
    private readonly IBillingService _billingService;
    private readonly IAppointmentService _appointmentService;
    private readonly IProcedureService _procedureService;
    private readonly IMapper _mapper;

    public BillingController(IBillingService billingService, IAppointmentService appointmentService, IProcedureService procedureService, IMapper mapper)
    {
        _billingService = billingService;
        _appointmentService = appointmentService;
        _procedureService = procedureService;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index(InvoiceStatus? status, DateTime? dateFrom, DateTime? dateTo, int pageNumber = 1, int pageSize = 10)
    {
        var result = await _billingService.GetAllAsync(pageNumber, pageSize);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Message;
            return View(new InvoiceListViewModel());
        }

        var invoices = result.Data;

        // Manual filtering since service doesn't have a Search method yet
        if (status.HasValue)
            invoices = invoices.Where(i => i.Status == status.Value);
        
        if (dateFrom.HasValue)
            invoices = invoices.Where(i => i.CreatedAt >= dateFrom.Value);
            
        if (dateTo.HasValue)
            invoices = invoices.Where(i => i.CreatedAt <= dateTo.Value);

        var statsResult = await _billingService.GetStatisticsAsync();
        
        var vm = new InvoiceListViewModel
        {
            Invoices = _mapper.Map<IEnumerable<InvoiceRowViewModel>>(invoices),
            Status = status,
            DateFrom = dateFrom,
            DateTo = dateTo,
            TotalCount = statsResult.IsSuccess ? statsResult.Data.TotalInvoices : 0,
            PaidCount = statsResult.IsSuccess ? statsResult.Data.PaidInvoices : 0,
            UnpaidCount = statsResult.IsSuccess ? statsResult.Data.UnpaidInvoices : 0,
            TotalRevenue = statsResult.IsSuccess ? statsResult.Data.TotalRevenue : 0
        };

        ViewBag.pageNumber = pageNumber;
        ViewBag.pageSize = pageSize;
        return View(vm);
    }

    // GET /Billing/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return BadRequest();

        var result = await _billingService.GetWithItemsAsync(id.Value);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        var vm = new InvoiceDetailsViewModel
        {
            Invoice = _mapper.Map<InvoiceRowViewModel>(result.Data),
            Items = _mapper.Map<IEnumerable<InvoiceItemDetailViewModel>>(result.Data.Items),
            AuditInfo = new AuditInfoViewModel
            {
                CreatedAt = result.Data.CreatedAt,
                CreatedByName = result.Data.CreatedBy,
                UpdatedAt = result.Data.UpdatedAt,
                UpdatedByName = result.Data.UpdatedBy
            }
        };

        return View(vm);
    }

    // GET /Billing/Create
    public async Task<IActionResult> Create()
    {
        var vm = new InvoiceFormViewModel();
        await PopulateDropdownsAsync(vm);
        return View(vm);
    }

    private async Task PopulateDropdownsAsync(InvoiceFormViewModel vm)
    {
        var appointmentsResult = await _appointmentService.GetAllAsync();
        if (!appointmentsResult.IsSuccess)
        {
            ModelState.AddModelError("", $"Error loading appointments: {appointmentsResult.Message}");
            vm.Appointments = new List<SelectListItem>();
        }
        else
        {
            vm.Appointments = appointmentsResult.Data
                .Where(a => a.Status != AppointmentStatus.Cancelled)
                .Select(a => new SelectListItem
                {
                    Text = $"{a.AppointmentDate:yyyy-MM-dd HH:mm} - {a.PatientName} | {a.DoctorName} (Status: {a.Status}, Invoiced: {a.HasInvoice})",
                    Value = a.Id.ToString()
                }).ToList();
        }

        var proceduresResult = await _procedureService.GetActiveTypesAsync();
        if (!proceduresResult.IsSuccess)
        {
            ModelState.AddModelError("", $"Error loading procedures: {proceduresResult.Message}");
            vm.ProcedureTypes = new List<SelectListItem>();
        }
        else
        {
            vm.ProcedureTypes = proceduresResult.Data.Select(p => new SelectListItem
            {
                Text = $"{p.Name} ({p.DefaultCost:N2} EGP)",
                Value = p.DefaultCost.ToString()
            }).ToList();
        }
    }

    // POST /Billing/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(InvoiceFormViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync(vm);
            return View(vm); 
        }

        var appointmentResult = await _appointmentService.GetByIdAsync(vm.AppointmentId);
        if (!appointmentResult.IsSuccess)
        {
            ModelState.AddModelError("AppointmentId", "Invalid appointment.");
            await PopulateDropdownsAsync(vm);
            return View(vm);
        }

        var dto = new CreateInvoiceDto
        {
            PatientId = appointmentResult.Data.PatientId,
            AppointmentId = vm.AppointmentId,
            Items = vm.Items.Select(i => new CreateInvoiceItemDto
            {
                Description = i.Description,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                ItemType = i.ItemType
            }).ToList()
        };

        var result = await _billingService.CreateAsync(dto);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            await PopulateDropdownsAsync(vm);
            return View(vm);
        }

        TempData["Success"] = "Invoice created successfully.";
        return RedirectToAction(nameof(Details), new { id = result.Data });
    }

    //Get/Billing/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null) return BadRequest();

        var result = await _billingService.GetWithItemsAsync(id.Value);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        var dto = new UpdateInvoiceDto
        {
            Id = result.Data.Id,
            Items = result.Data.Items.Select(i => new CreateInvoiceItemDto()
            {
                Description = i.Description,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                ItemType = i.ItemType

            }).ToList()

        };
        return View(dto);
    }

    //Post/Billing/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateInvoiceDto dto)
    {
        if (id != dto.Id) return BadRequest();
        if (!ModelState.IsValid)
            return View(dto);

        var result = await _billingService.UpdateAsync(dto);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(dto);
        }

        TempData["Success"] = result.Message;
        return RedirectToAction(nameof(Details), new { id });
    }

    // POST /Billing/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _billingService.DeleteAsync(id);
        TempData[result.IsSuccess ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(Index));

    }

    // GET /Billing/DailyReport
    [HttpGet]
    public async Task<IActionResult> DailyReport()
    {
        // Use a range that covers today in both local and UTC to avoid empty reports
        var start = DateTime.Today.AddDays(-1); // Start from yesterday to be safe
        var end = DateTime.Today.AddDays(1).AddTicks(-1);

        var result = await _billingService.GetStatisticsByDateRangeAsync(start, end);
        var stats = result.IsSuccess ? result.Data : new BillingStatisticsDto();

        var vm = new DailyReportViewModel
        {
            SelectedDate = start,
            TotalRevenue = stats.TotalRevenue,
            TotalInvoices = stats.TotalInvoices,
            PaidCount = stats.PaidInvoices,
            UnpaidCount = stats.UnpaidInvoices,
            UnpaidAmount = stats.UnpaidRevenue
        };

        ViewBag.From = start;
        ViewBag.To = end;
        return View(vm);
    }

    // POST /Billing/DailyReport
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DailyReport(DateTime? from, DateTime? to)
    {
        var start = from ?? DateTime.Today.AddDays(-1);
        var end = to ?? DateTime.Today.AddDays(1).AddTicks(-1);

        var result = await _billingService.GetStatisticsByDateRangeAsync(start, end);
        var stats = result.IsSuccess ? result.Data : new BillingStatisticsDto();

        var vm = new DailyReportViewModel
        {
            SelectedDate = start,
            TotalRevenue = stats.TotalRevenue,
            TotalInvoices = stats.TotalInvoices,
            PaidCount = stats.PaidInvoices,
            UnpaidCount = stats.UnpaidInvoices,
            UnpaidAmount = stats.UnpaidRevenue
        };

        if (!result.IsSuccess)
            TempData["Error"] = result.Message;

        ViewBag.From = start;
        ViewBag.To = end;
        return View(vm);
    }

    //Get/Billing/ByPatient/5
    public async Task<IActionResult> ByPatient(int patientId)
    {
        var result = await _billingService.GetByPatientAsync(patientId);
        if (!result.IsSuccess)        {
            TempData["Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        ViewBag.PatientId = patientId;
        return View(result.Data);
    }
    
    //Get/Billing/Unpaid
    public async Task<IActionResult> Unpaid()
    {
        var result = await _billingService.GetUnpaidAsync();
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        return View(result.Data);
    }
    
    // Status Action
    // GET /Billing/MarkAsPaid/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsPaid(int id)
    {
        var result = await _billingService.MarkAsPaidAsync(id);
        TempData[result.IsSuccess ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(Details), new { id });
    }
    
    // GET /Billing/MarkAsPartiallyPaid/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsPartiallyPaid(int id)
    {
        var result = await _billingService.MarkAsPartiallyPaidAsync(id);
        TempData[result.IsSuccess ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(Details), new { id });
    }
    
    // GET /Billing/GetStatistics  (AJAX)
    [HttpGet]
    public async Task<IActionResult> GetStatistics()
    {
        var result = await _billingService.GetStatisticsAsync();
        if (!result.IsSuccess)
            return Json(new { success = false, message = result.Message });

        return Json(new { success = true, data = result.Data });
    }
    
    // GET /Billing/GetTotalRevenue  (AJAX)
    [HttpPost]
    public async Task<IActionResult> GetTotalRevenue()
    {
        var result = await _billingService.GetTotalRevenueAsync();
        if (!result.IsSuccess)
            return Json(new { success = false, message = result.Message });

        return Json(new { success = true, totalRevenue = result.Data });
    }
}
