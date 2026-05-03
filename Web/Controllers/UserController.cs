using Microsoft.AspNetCore.Mvc;

public class UserController : Controller
{
    public IActionResult Index() => View();
    public IActionResult Create() => View();
    public IActionResult Edit(int? id) => View();
    public IActionResult ResetPassword(int? id) => View();
}
