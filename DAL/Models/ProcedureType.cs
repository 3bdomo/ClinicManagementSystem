namespace ClinicSystem.DAL.Models;
using Common.Interfaces;
public class ProcedureType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal DefaultCost { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public ICollection<Procedure> Procedures { get; set; } = new List<Procedure>();
}