using Microsoft.AspNetCore.Mvc;

public class DoctorController : Controller
{
    public IActionResult Index() => View();
    public IActionResult Details(int? id) => View();
    public IActionResult Edit(int? id) => View();
}
