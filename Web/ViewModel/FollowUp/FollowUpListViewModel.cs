using Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModel;
public class FollowUpListViewModel
{
    public IEnumerable<MedicalRecordRowViewModel> MedicalRecords { get; set; } = [];
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public int TotalCount { get; set; }
}
