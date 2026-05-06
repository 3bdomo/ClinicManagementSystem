using BLL.DTOs.Auth;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.ViewModel;
using Common.Enums;
using System.Security.Claims;

namespace Web.Controllers;

public class AccountController : Controller
{
    private readonly IAuthService _authService;

    public AccountController(IAuthService authService)
    {
        _authService = authService;
    }

    public IActionResult Login() => View();
    public IActionResult Register() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

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
            BloodType = model.BloodType.HasValue ? model.BloodType.Value switch
            {
                BloodType.A_Positive => "A+", BloodType.A_Negative => "A-",
                BloodType.B_Positive => "B+", BloodType.B_Negative => "B-",
                BloodType.AB_Positive => "AB+", BloodType.AB_Negative => "AB-",
                BloodType.O_Positive => "O+", BloodType.O_Negative => "O-",
                _ => model.BloodType.Value.ToString()
            } : null,
            EmergencyContact = model.EmergencyContact
        };

        var result = await _authService.RegisterAsync(dto, UserRole.Patient);
        if (result.IsSuccess)
        {
            return RedirectToAction("Login");
        }
        
        ModelState.AddModelError(string.Empty, result.Message);
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
}
