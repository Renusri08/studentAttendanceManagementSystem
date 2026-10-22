using System.ComponentModel.DataAnnotations;

namespace StudentAttendanceManagement.ViewModels;

public class LoginViewModel 
{   
    [Required(ErrorMessage="Username or Email is required.")]
    public string UsernameOrEmail{get;set;}

    [Required(ErrorMessage="PIN is required.")]
    [RegularExpression(@"^\d+$",
        ErrorMessage = "PIN contains only digits and minimum length of 4")]
    public String Pin{get;set;}

   
}