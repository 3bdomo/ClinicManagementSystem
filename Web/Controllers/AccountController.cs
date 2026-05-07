using BLL.DTOs.Auth;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.ViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Common.Enums;

namespace Web.Controllers;

public class AccountController : Controller
{
    private readonly IAuthService _authService;

    public AccountController(IAuthService authService)
    {
        _authService = authService;
    }

    public IActionResult Login() => View();
    public IActionResult Register()
    {
        ViewBag.BloodTypes = GetBloodTypes();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.BloodTypes = GetBloodTypes();
            return View(model);
        }

        var dto = new RegisterDto
        {
            FullName = model.FullName,
            Email = model.Email,
            Password = model.Password,
            NationalId = model.NationalId,
            Phone = model.Phone,
            DateOfBirth = model.DateOfBirth,
            Gender = model.Gender,
            Address = model.Address,
            BloodType = model.BloodType,
            EmergencyContact = model.EmergencyContact
        };

        var result = await _authService.RegisterAsync(dto, UserRole.Patient);
        if (result.IsSuccess)
        {
            return RedirectToAction("Login");
        }
        
        ModelState.AddModelError(string.Empty, result.Message);
        ViewBag.BloodTypes = GetBloodTypes();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _authService.LoginAsync(new LoginDto { Email = model.Email, Password = model.Password, RememberMe = model.RememberMe });
        if (result.IsSuccess)
        {
            if (!result.Data.IsInRole(UserRole.Patient.ToString()))
            {
                ModelState.AddModelError(string.Empty, "Access denied. You do not have patient privileges.");
                return View(model);
            }
            await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, result.Data, new AuthenticationProperties
            {
                IsPersistent = model.RememberMe
            });
            return RedirectToAction("Dashboard", "Home");
        }
        
        ModelState.AddModelError(string.Empty, result.Message);
        return View(model);
    }

    [HttpGet]
    public IActionResult LoginDoctor() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LoginDoctor(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _authService.LoginAsync(new LoginDto { Email = model.Email, Password = model.Password, RememberMe = model.RememberMe });
        if (result.IsSuccess)
        {
            if (!result.Data.IsInRole(UserRole.Doctor.ToString()) && !result.Data.IsInRole(UserRole.Receptionist.ToString()))
            {
                ModelState.AddModelError(string.Empty, "Access denied. You do not have doctor or receptionist privileges.");
                return View(model);
            }

            await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, result.Data, new AuthenticationProperties { IsPersistent = model.RememberMe });
            return RedirectToAction("Dashboard", "Home");
        }
        
        ModelState.AddModelError(string.Empty, result.Message);
        return View(model);
    }

    [HttpGet]
    public IActionResult AdminLogin() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AdminLogin(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _authService.LoginAsync(new LoginDto { Email = model.Email, Password = model.Password, RememberMe = model.RememberMe });
        if (result.IsSuccess)
        {
            if (!result.Data.IsInRole(UserRole.Admin.ToString()))
            {
                ModelState.AddModelError(string.Empty, "Access denied. You do not have administrator privileges.");
                return View(model);
            }

            await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, result.Data, new AuthenticationProperties { IsPersistent = model.RememberMe });
            return RedirectToAction("Dashboard", "Home");
        }
        
        ModelState.AddModelError(string.Empty, result.Message);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult MyProfile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var fullName = User.Identity?.Name;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        return Json(new { UserId = userId, FullName = fullName, Role = role });
    }
    [HttpGet]
    public IActionResult AccessDenied() => View();

    private List<SelectListItem> GetBloodTypes()
    {
        return new List<SelectListItem>
        {
            new SelectListItem { Text = "A+", Value = "A+" },
            new SelectListItem { Text = "A-", Value = "A-" },
            new SelectListItem { Text = "B+", Value = "B+" },
            new SelectListItem { Text = "B-", Value = "B-" },
            new SelectListItem { Text = "AB+", Value = "AB+" },
            new SelectListItem { Text = "AB-", Value = "AB-" },
            new SelectListItem { Text = "O+", Value = "O+" },
            new SelectListItem { Text = "O-", Value = "O-" }
        };
    }
}
