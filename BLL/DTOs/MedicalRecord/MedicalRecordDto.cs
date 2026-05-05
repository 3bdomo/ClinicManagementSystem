using System.Runtime.InteropServices.JavaScript;

namespace BLL.DTOs.MedicalRecord
{
    public class MedicalRecordDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string? PatientName { get; set; }
        public int DoctorId { get; set; }
        public string? DoctorName { get; set; }
        public int AppointmentId { get; set; }
        public string? Diagnosis { get; set; }
        public string? Notes { get; set; }
        public DateTime VisitedDate { get; set; }
        public DateTime? FollowUpDate { get; set; }
        public int ProceduresCount { get; set; }
        public int AttachmentsCount { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        
    }
}

