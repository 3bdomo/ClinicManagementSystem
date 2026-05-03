using Microsoft.AspNetCore.Mvc;

public class DoctorScheduleController : Controller
{
    public IActionResult Index() => View();
    public IActionResult Create() => View();
    public IActionResult Edit(int? id) => View();
}
