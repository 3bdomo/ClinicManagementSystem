using BLL.DTOs.Billing;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

public class BillingController : Controller
{
    private readonly IBillingService _billingService;

    public BillingController(IBillingService billingService)
    {
        _billingService = billingService;
    }

    public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
    {
        var result = await _billingService.GetAllAsync(pageNumber, pageSize);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Message;
            return View(new List<InvoiceDto>());
        }

        var statusResult = await _billingService.GetStatisticsAsync();
        if (statusResult.IsSuccess)
            ViewBag.Statistics = statusResult.Data;


        ViewBag.pageNumber = pageNumber;
        ViewBag.pageSize = pageSize;
        return View(result.Data);

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

        return View(result.Data);
    }

    // GET /Billing/Create
    public IActionResult Create() => View(new CreateInvoiceDto());

    // POST /Billing/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateInvoiceDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var result = await _billingService.CreateAsync(dto);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(dto);
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

    // POST /Billing/DailyReport
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DailyReport(DateTime? from, DateTime? to)
    {
        var start = from ?? DateTime.Today;
        var end = to ?? DateTime.Today.AddDays(1).AddTicks(-1);

        var result = await _billingService.GetStatisticsByDateRangeAsync(start, end);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Message;
            return View(new BillingStatisticsDto());
        }

        ViewBag.From = start;
        ViewBag.To = end;
        return View(result.Data);
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
