using Microsoft.AspNetCore.Mvc;

public class PatientController : Controller
{
    public IActionResult Index() => View();
    public IActionResult Create() => View();
    public IActionResult Edit(int? id) => View();
    public IActionResult Details(int? id) => View();
    public IActionResult Deleted() => View();
    public IActionResult MyProfile() => View();
}
