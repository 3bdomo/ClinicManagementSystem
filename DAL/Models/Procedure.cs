using Common.Interfaces;
namespace ClinicSystem.DAL.Models;

public class Procedure : IAuditable
{
    public int Id { get; set; }
    public int MedicalRecordId { get; set; }
    public int ProcedureTypeId { get; set; }
    public DateTime PerformedAt { get; set; }
    public int? DurationMinutes { get; set; }
    public string? Notes { get; set; }
    public string? AfterCareNotes { get; set; }
    public decimal Cost { get; set; }

    // IAuditable
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    public MedicalRecord MedicalRecord { get; set; } = null!;
    public ProcedureType ProcedureType { get; set; } = null!;
}