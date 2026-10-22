namespace StudentAttendanceManagement.Models;


public class Student
{
    public int StudentId{get;set;}

    public string RollNumber{get;set;} = string.Empty;
    
    public string StudentName{get;set;} = string.Empty;

    public int DepartmentId{get;set;}

    public int YearOfStudy{get;set;}

    public bool IsActive{get;set;} = true;
    

    //navigation properties
    public Department Department{get;set;} = null;

    public ICollection<Attendance> Attendances{get;set;} = new List<Attendance>();


}