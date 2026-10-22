using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace StudentAttendanceManagement.ViewModels;

public class AttendanceViewModel
{
    [Required(ErrorMessage = "Please select a department.")]
    public string? DepartmentName { get; set; }

    [Required(ErrorMessage = "Please select a year.")]
    [Range(1, 4, ErrorMessage = "Select a valid year.")]
    public int? YearOfStudy { get; set; }

    public List<SelectListItem> Departments { get; set; } = new();

    public List<StudentAttendanceItemViewModel> Students { get; set; } = new();
}


public class StudentAttendanceItemViewModel
{
    public int StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public string RollNumber { get; set; } = string.Empty;

    public string DepartmentName { get; set; } = string.Empty;

    public int YearOfStudy { get; set; }
}

