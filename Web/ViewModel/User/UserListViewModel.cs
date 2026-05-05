using Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModel;
public class UserListViewModel
{
    public IEnumerable<UserRowViewModel> Users { get; set; } = [];
    public string? RoleFilter { get; set; }
    public string? SearchQuery { get; set; }
}
