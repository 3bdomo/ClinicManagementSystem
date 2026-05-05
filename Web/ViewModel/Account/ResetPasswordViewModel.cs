using Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModel;
public class ResetPasswordViewModel
{
    public string UserId { get; set; } = string.Empty;
    [Required, DataType(DataType.Password), MinLength(8)] public string NewPassword { get; set; } = string.Empty;
    [Compare(nameof(NewPassword)), DataType(DataType.Password)] public string ConfirmPassword { get; set; } = string.Empty;
}
