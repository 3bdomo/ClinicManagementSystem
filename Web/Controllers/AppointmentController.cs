using Microsoft.AspNetCore.Mvc;

public class AppointmentController : Controller
{
    public IActionResult Index() => View();
    public IActionResult Calendar() => View();
    public IActionResult Book() => View();
    public IActionResult BookSlot() => View();
    public IActionResult MyAppointments() => View();
    [HttpGet]
    public IActionResult GetAvailableSlots(int doctorId, string date)
        => Json(new[] { "09:00", "09:30", "10:00", "10:30", "11:00", "11:30", "12:00" });
    [HttpGet]
    public IActionResult GetCalendarEvents()
        => Json(new[] { new { title = "Check-up", start = DateTime.Today.ToString("yyyy-MM-dd"), type = "Checkup" } });
}
