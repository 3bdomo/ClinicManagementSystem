using Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModel;
public class PatientListViewModel
{
    public IEnumerable<PatientRowViewModel> Patients { get; set; } = [];
    public string? SearchQuery { get; set; }
}
