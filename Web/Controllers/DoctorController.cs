using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
public class DoctorController : Controller
{
    public IActionResult Index() => View();
    public IActionResult Details(int? id) => View();
    public IActionResult Edit(int? id) => View();
}
