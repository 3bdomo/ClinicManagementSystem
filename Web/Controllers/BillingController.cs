using Microsoft.AspNetCore.Mvc;

public class BillingController : Controller
{
    public IActionResult Index() => View();
    public IActionResult Create() => View();
    public IActionResult Details(int? id) => View();
    public IActionResult DailyReport() => View();
}
