using BLL.DTOs.Receptionist;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.ViewModels.Receptionist;

namespace Web.Controllers;






public class ReceptionistController : Controller
{
    private readonly IReceptionistService _receptionistService;

    public ReceptionistController(IReceptionistService receptionistService)
    {
        _receptionistService = receptionistService;
    }

    
    
    

    public async Task<IActionResult> Index()
    {
        var result = await _receptionistService.GetAllAsync();

        
        var vm = new ReceptionistListViewModel
        {
            Receptionists = result.Data ?? Enumerable.Empty<ReceptionistDto>()
        };

        return View(vm);
    }

    
    
    

    public async Task<IActionResult> Details(int id)
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

        return View(new ReceptionistDetailsViewModel
        {
            Receptionist = result.Data!
        });
    }

    
    
    

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
            Email = data.Email,       
            PhoneNumber = data.PhoneNumber,
            IsActive = data.IsActive
        });
    }

    
    
    

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ReceptionistFormViewModel vm)
    {
        
        if (id != vm.Id)
        {
            TempData["Error"] = "Request mismatch. Please try again.";
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
            return View(vm);

        
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

        
        TempData["Error"] = result.Message;
        return View(vm);
    }

    
    
    

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

        
        return returnTo == "Details"
            ? RedirectToAction(nameof(Details), new { id })
            : RedirectToAction(nameof(Index));
    }
}