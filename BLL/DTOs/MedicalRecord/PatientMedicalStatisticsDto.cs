namespace BLL.DTOs.MedicalRecord
{
    public class PatientMedicalStatisticsDto
    {
    public int PatientId { get; set; }
    public int TotalRecords { get; set; }
    public int TotalVisits { get; set; }
    public int TotalProcedures{ get; set; }
    public int UniqueConditions { get; set; }
    public string? LatestDiagnosis { get; set; }
    public DateTime? LastVisitDate { get; set; }
    public int PendingFollowUps { get; set; }
    public DateTime? FirstRecordDate { get; set; }
    public double AverageVisitsPerRecord { get; set; }
    }
}

