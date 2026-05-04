using BLL.DTOs.Patient;
using BLL.Services;
using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    PatientAccountService _service;
    public AccountController(PatientAccountService service)    {
        _service = service;
    }
    public IActionResult Login() => View();
    public IActionResult Register() => View();

    [HttpPost]
    public IActionResult Register(PatientRegisterDto user)
    {
       // BLL.Services.PatientAccountService service = new BLL.Services.PatientAccountService();
        var result = _service.RegisterAsync(user).Result;
        if (result.IsSuccess)
        {
            return RedirectToAction("Login");
        }
        ModelState.AddModelError(string.Empty, result?.Message);
        return View(user);
    }
}
