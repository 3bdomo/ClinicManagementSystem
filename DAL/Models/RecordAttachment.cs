namespace ClinicSystem.DAL.Models;

public class RecordAttachment
{
    public int Id { get; set; }
    public int MedicalRecordId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public string? UploadedBy { get; set; }

    // Navigation Properties
    public MedicalRecord MedicalRecord { get; set; } = null!;
}