using Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModel;
public class DeletedPatientsViewModel
{
    public IEnumerable<PatientRowViewModel> DeletedPatients { get; set; } = [];
}
