
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentAttendanceManagement.Data;
using StudentAttendanceManagement.ViewModels;
using StudentAttendanceManagement.Models;
using System.Security.Claims;

namespace StudentAttendanceManagement.Controllers;

[Authorize]
public class AttendanceController : Controller
{
    private readonly AppDbContext _context;

    public AttendanceController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? departmentName,
        int? yearOfStudy)
    {
        var model = new AttendanceViewModel
        {
            DepartmentName = departmentName,
            YearOfStudy = yearOfStudy,
            Departments = await _context.Departments
                .Where(d => d.IsActive)
                .OrderBy(d => d.DepartmentName)
                .Select(d => new SelectListItem
                {
                    Text = d.DepartmentName,
                    Value = d.DepartmentName
                })
                .ToListAsync()
        };

        // Retrieve students only after both filters are selected.
        if (!string.IsNullOrWhiteSpace(departmentName)
            && yearOfStudy.HasValue
            && yearOfStudy.Value >= 1
            && yearOfStudy.Value <= 4)
        {
            model.Students = await _context.Students
                .Where(s =>
                    s.IsActive &&
                    s.Department.DepartmentName == departmentName &&
                    s.YearOfStudy == yearOfStudy.Value)
                .OrderBy(s => s.RollNumber)
                .Select(s => new StudentAttendanceItemViewModel
                {
                    StudentId = s.StudentId,
                    StudentName = s.StudentName,
                    RollNumber = s.RollNumber,
                    DepartmentName = s.Department.DepartmentName,
                    YearOfStudy = s.YearOfStudy
                })
                .ToListAsync();
        }

        return View(model);
    }

    //Display Mark attendance Page for a specific student
    [HttpGet]
    public async Task<IActionResult> Mark(int studentId)
    {
        var student = await _context.Students
            .Include(s => s.Department)
            .FirstOrDefaultAsync(s =>
                s.StudentId == studentId && s.IsActive);

        if (student == null)
        {
            return NotFound();
        }

        bool alreadySubmitted = await _context.Attendance
            .AnyAsync(a =>
                a.StudentId == student.StudentId &&
                a.AttendanceDate == DateTime.Today);

        if (alreadySubmitted)
        {
            TempData["DuplicateAttendanceMessage"] =
                "Attendance for this student has already been submitted today.";

            return RedirectToAction(nameof(Index), new
            {
                departmentName = student.Department.DepartmentName,
                yearOfStudy = student.YearOfStudy
            });
        }

        var model = new MarkAttendanceViewModel
        {
            StudentId = student.StudentId,
            StudentName = student.StudentName,
            RollNumber = student.RollNumber,
            DepartmentName = student.Department.DepartmentName,
            YearOfStudy = student.YearOfStudy
        };

        return View(model);
    }

    //Process the submitted Mark Attendance Form
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Mark(MarkAttendanceViewModel model)
    {
        // Find the student in the database.
        var student = await _context.Students
            .Include(s => s.Department)
            .FirstOrDefaultAsync(s =>
                s.StudentId == model.StudentId && s.IsActive);

        if (student == null)
        {
            return NotFound();
        }

        var today = DateTime.Today;
        bool alreadySubmitted = await _context.Attendance
            .AnyAsync(a =>
                a.StudentId == student.StudentId &&
                a.AttendanceDate == today);

        if (alreadySubmitted)
        {
            TempData["DuplicateAttendanceMessage"] =
                "Attendance for this student has already been submitted today.";

            return RedirectToAction(nameof(Index), new
            {
                departmentName = student.Department.DepartmentName,
                yearOfStudy = student.YearOfStudy
            });
        }

        // Validate the attendance status.
        if (model.AttendanceStatus != "Present" &&
            model.AttendanceStatus != "Absent")
        {
            ModelState.AddModelError(
                nameof(model.AttendanceStatus),
                "Please select Present or Absent.");
        }
        
        model.StudentId = student.StudentId;
        model.StudentName = student.StudentName;
        model.RollNumber = student.RollNumber;
        model.DepartmentName = student.Department.DepartmentName;
        model.YearOfStudy = student.YearOfStudy;
       

        // If validation fails, display the form again.
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Get the logged-in user's ID from their claims.
        var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdValue, out int userId))
        {
            return Forbid();
        }

        // Create the attendance record.
        var attendance = new Attendance
        {
            StudentId = student.StudentId,
            AttendanceDate = today,
            AttendanceStatus = model.AttendanceStatus,
            MarkedByUserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        // Save the record to SQL Server.
        _context.Attendance.Add(attendance);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            TempData["DuplicateAttendanceMessage"] =
                "Attendance for this student has already been submitted today.";

            return RedirectToAction(nameof(Index), new
            {
                departmentName = student.Department.DepartmentName,
                yearOfStudy = student.YearOfStudy
            });
        }

        // Redirect after successful submission.
        TempData["SuccessMessage"] = "Attendance saved successfully.";

        return RedirectToAction(nameof(Index), new
        {
            departmentName = student.Department.DepartmentName,
            yearOfStudy = student.YearOfStudy
        });
    }


}
