using BLL.DTOs.Receptionist;

namespace Web.ViewModels.Receptionist;

public class ReceptionistListViewModel
{
    public IEnumerable<ReceptionistDto> Receptionists { get; set; }
        = Enumerable.Empty<ReceptionistDto>();

    
    public int TotalCount => Receptionists.Count();
    public int ActiveCount => Receptionists.Count(r => r.IsActive);
    public int InactiveCount => Receptionists.Count(r => !r.IsActive);
}