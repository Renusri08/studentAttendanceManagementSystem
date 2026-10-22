
using System.ComponentModel.DataAnnotations;

namespace StudentAttendanceManagement.ViewModels;

public class SignupViewModel
{
    [Required(ErrorMessage = "Full name is required.")]
    [RegularExpression(@"^[A-Za-z ]+$", ErrorMessage = "Full name must contain only letters and spaces.")]
    [StringLength(50)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    [StringLength(50)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Username is required.")]
    [StringLength(50, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "PIN is required.")]
    [RegularExpression(@"^\d+$",
        ErrorMessage = "PIN must contain only digits.")]
    [StringLength(int.MaxValue,MinimumLength=4,ErrorMessage="PIN must contain atleast 4 digits.")]
    public string Pin { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please confirm your PIN.")]
    [Compare(nameof(Pin),
        ErrorMessage = "PIN and Confirm PIN must match.")]
    public string ConfirmPin { get; set; } = string.Empty;
}
