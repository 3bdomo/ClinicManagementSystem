using BLL.DTOs.Receptionist;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.ViewModels.Receptionist;

namespace Web.Controllers;

/// <summary>
/// Manages receptionist profiles — Admin only.
/// Create / Delete go through UserController (same pattern as Doctor).
/// </summary>
//[Authorize(Roles = "Admin")]
public class ReceptionistController : Controller
{
    private readonly IReceptionistService _receptionistService;

    public ReceptionistController(IReceptionistService receptionistService)
    {
        _receptionistService = receptionistService;
    }

    // ══════════════════════════════════════════════════════════════
    // INDEX  —  GET /Receptionist
    // ══════════════════════════════════════════════════════════════

    public async Task<IActionResult> Index()
    {
        var result = await _receptionistService.GetAllAsync();

        // Service never returns null Data on success, but guard anyway
        var vm = new ReceptionistListViewModel
        {
            Receptionists = result.Data ?? Enumerable.Empty<ReceptionistDto>()
        };

        return View(vm);
    }

    // ══════════════════════════════════════════════════════════════
    // DETAILS  —  GET /Receptionist/Details/{id}
    // ══════════════════════════════════════════════════════════════

    public async Task<IActionResult> Details(int id)
    {
        // Basic route guard — service also validates, but fail fast
        if (id <= 0)
        {
            TempData["Error"] = "Invalid receptionist ID.";
            return RedirectToAction(nameof(Index));
        }

        var result = await _receptionistService.GetByIdAsync(id);

        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        return View(new ReceptionistDetailsViewModel
        {
            Receptionist = result.Data!
        });
    }

    // ══════════════════════════════════════════════════════════════
    // EDIT  —  GET /Receptionist/Edit/{id}
    // ══════════════════════════════════════════════════════════════

    public async Task<IActionResult> Edit(int id)
    {
        if (id <= 0)
        {
            TempData["Error"] = "Invalid receptionist ID.";
            return RedirectToAction(nameof(Index));
        }

        var result = await _receptionistService.GetByIdAsync(id);

        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        var data = result.Data!;

        return View(new ReceptionistFormViewModel
        {
            Id = data.Id,
            FullName = data.FullName,
            Email = data.Email,       // display-only, not bound on POST
            PhoneNumber = data.PhoneNumber,
            IsActive = data.IsActive
        });
    }

    // ══════════════════════════════════════════════════════════════
    // EDIT  —  POST /Receptionist/Edit/{id}
    // ══════════════════════════════════════════════════════════════

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ReceptionistFormViewModel vm)
    {
        // Ensure route id matches posted Id (prevent tampering)
        if (id != vm.Id)
        {
            TempData["Error"] = "Request mismatch. Please try again.";
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
            return View(vm);

        // Trim whitespace before sending to service
        vm.FullName = vm.FullName.Trim();
        vm.PhoneNumber = string.IsNullOrWhiteSpace(vm.PhoneNumber)
                         ? null
                         : vm.PhoneNumber.Trim();

        var dto = new UpdateReceptionistDto
        {
            Id = vm.Id,
            FullName = vm.FullName,
            PhoneNumber = vm.PhoneNumber,
            IsActive = vm.IsActive
        };

        var result = await _receptionistService.UpdateAsync(dto);

        if (result.IsSuccess)
        {
            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Details), new { id = vm.Id });
        }

        // Service-level error (e.g. DB failure) — show on same form
        TempData["Error"] = result.Message;
        return View(vm);
    }

    // ══════════════════════════════════════════════════════════════
    // TOGGLE ACTIVE  —  POST /Receptionist/ToggleActive/{id}
    // ══════════════════════════════════════════════════════════════

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(int id, string? returnTo = null)
    {
        if (id <= 0)
        {
            TempData["Error"] = "Invalid receptionist ID.";
            return RedirectToAction(nameof(Index));
        }

        var result = await _receptionistService.ToggleActiveAsync(id);
        TempData[result.IsSuccess ? "Success" : "Error"] = result.Message;

        // Return to Details if toggled from there, otherwise Index
        return returnTo == "Details"
            ? RedirectToAction(nameof(Details), new { id })
            : RedirectToAction(nameof(Index));
    }
}