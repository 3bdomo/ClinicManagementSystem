using Microsoft.AspNetCore.Mvc;

public class ProcedureController : Controller
{
    public IActionResult ManageTypes() => View();
    public IActionResult CreateType() => View();
    public IActionResult EditType(int? id) => View();
    public IActionResult Create() => View();
    public IActionResult Edit(int? id) => View();
    public IActionResult Details(int? id) => View();
}
