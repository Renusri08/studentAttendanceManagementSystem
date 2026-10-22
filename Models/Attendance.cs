namespace StudentAttendanceManagement.Models;

public class Attendance
{
    public int AttendanceId { get; set; }

    public int StudentId { get; set; }

    public DateTime AttendanceDate { get; set; }

    public string AttendanceStatus { get; set; } = string.Empty;

    public int MarkedByUserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Student Student { get; set; } = null!;

    public User MarkedByUser { get; set; } = null!;
}
