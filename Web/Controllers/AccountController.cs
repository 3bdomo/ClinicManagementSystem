using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    public IActionResult Login() => View();
    public IActionResult Register() => View();
}
