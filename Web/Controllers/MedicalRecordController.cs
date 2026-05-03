using Microsoft.AspNetCore.Mvc;

public class MedicalRecordController : Controller
{
    public IActionResult Details(int? id) => View();
    public IActionResult Create(int? patientId) => View();
    public IActionResult Edit(int? id) => View();
}
