using Common.Interfaces;
using Common.Enums;

namespace ClinicSystem.DAL.Models;

public class Invoice : IAuditable
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int AppointmentId { get; set; }
    public decimal TotalAmount { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Unpaid;
    public DateTime? PaidAt { get; set; }

    // IAuditable
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    public Patient Patient { get; set; } = null!;
    public Appointment Appointment { get; set; } = null!;
    public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
}