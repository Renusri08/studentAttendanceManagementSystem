
using System.ComponentModel.DataAnnotations;

namespace StudentAttendanceManagement.ViewModels;

public class MarkAttendanceViewModel
{
    // Student details
    public int StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public string RollNumber { get; set; } = string.Empty;

    public string DepartmentName { get; set; } = string.Empty;

    public int YearOfStudy { get; set; }

    // Attendance details
    [Required(ErrorMessage = "Please select attendance status.")]
    public string AttendanceStatus { get; set; } = string.Empty;
}