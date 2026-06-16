using System.ComponentModel.DataAnnotations;

namespace LifeAdvisor.Web.Models;

public class RegisterViewModel
{
    [Required]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Date of birth")]
    public DateOnly DateOfBirth { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddYears(-18));

    [Required]
    public string City { get; set; } = string.Empty;

    [Required]
    public string Country { get; set; } = string.Empty;
}

public class LoginViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
