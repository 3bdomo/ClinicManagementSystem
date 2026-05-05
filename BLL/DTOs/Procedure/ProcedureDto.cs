namespace BLL.DTOs.Procedure
{
    public class ProcedureDto
    {
        public int Id { get; set; }
        public int MedicalRecordId { get; set; }
        public int ProcedureTypeId { get; set; }
        public string? ProcedureTypeName { get; set; }
        public DateTime PerformedAt { get; set; }
        public int? DurationMinutes { get; set; }
        public string? Notes { get; set; }
        public string? AfterCareNotes { get; set; }
        public decimal Cost { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

