using Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModel;
public class PatientRowViewModel
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public int Age => DateOnly.FromDateTime(DateTime.Today).Year - DateOfBirth.Year -
                     (DateOnly.FromDateTime(DateTime.Today).DayOfYear < DateOfBirth.DayOfYear ? 1 : 0);
    public Gender Gender { get; set; }
    public string NationalId { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public bool HasPortalAccount { get; set; }
    public string? BloodType { get; set; }
    public string? Address { get; set; }
    public string? EmergencyContact { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}
