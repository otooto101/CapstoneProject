using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LifeAdvisor.Web.Models;

public class OnboardingViewModel
{
    [Required]
    [Display(Name = "Preferred name")]
    public string PreferredName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Date of birth")]
    public DateOnly DateOfBirth { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddYears(-18));

    [Required]
    public string City { get; set; } = string.Empty;

    [Required]
    public string Country { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Life stage")]
    [Range(1, int.MaxValue, ErrorMessage = "Please choose a life stage.")]
    public int LifeStageOptionId { get; set; }

    public List<SelectListItem> LifeStageOptions { get; set; } = [];
}
