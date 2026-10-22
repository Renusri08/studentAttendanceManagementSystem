namespace StudentAttendanceManagement.Models;

public class Department
{
    public int DepartmentId{get;set;}

    
    public string DepartmentName{get;set;} = string.Empty;
    
    public bool IsActive{get;set;} = true;

    //navigation property
    public ICollection<Student> Students{get;set;} = new List<Student>();
}