namespace BLL.DTOs.MedicalRecord
{

    public class RecordAttachmentDto
    {
        public int Id { get; set; }
        public int MedicalRecordId { get; set; }
        public string FileName { get; set; }= string.Empty;
        public string FileType { get; set; }= string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string? Description { get; set; }
        public string? UploadedBy { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}