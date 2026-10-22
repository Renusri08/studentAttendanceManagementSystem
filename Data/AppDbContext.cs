using Microsoft.EntityFrameworkCore;
using StudentAttendanceManagement.Models;

namespace StudentAttendanceManagement.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    public DbSet<Department> Departments { get; set; }

    public DbSet<Student> Students { get; set; }

    public DbSet<Attendance> Attendance { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User constraints
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.UserId);

            entity.Property(u => u.FullName)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(u => u.Email)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(u => u.Username)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(u => u.PinHash)
                .HasMaxLength(300)
                .IsRequired();

            entity.HasIndex(u => u.Email)
                .IsUnique();

            entity.HasIndex(u => u.Username)
                .IsUnique();
        });

        // Department constraints
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(d => d.DepartmentId);

            entity.Property(d => d.DepartmentName)
                .HasMaxLength(100)
                .IsRequired();

            entity.HasIndex(d => d.DepartmentName)
                .IsUnique();
        });

        // Student constraints
        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(s => s.StudentId);

            entity.Property(s => s.RollNumber)
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(s => s.StudentName)
                .HasMaxLength(100)
                .IsRequired();

            entity.HasIndex(s => s.RollNumber)
                .IsUnique();

            entity.ToTable(t =>
                t.HasCheckConstraint(
                    "CK_Students_YearOfStudy",
                    "[YearOfStudy] BETWEEN 1 AND 4"));

            entity.HasOne(s => s.Department)
                .WithMany(d => d.Students)
                .HasForeignKey(s => s.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Attendance constraints
        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.HasKey(a => a.AttendanceId);

            entity.Property(a => a.AttendanceDate)
                .HasColumnType("date");

            entity.Property(a => a.AttendanceStatus)
                .HasMaxLength(10)
                .IsRequired();

            entity.Property(a => a.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.ToTable(t =>
                t.HasCheckConstraint(
                    "CK_Attendance_Status",
                    "[AttendanceStatus] IN ('Present', 'Absent')"));

            // A student can have only one attendance
            // record for each date.
            entity.HasIndex(a => new
                {
                    a.StudentId,
                    a.AttendanceDate
                })
                .IsUnique();

            entity.HasOne(a => a.Student)
                .WithMany(s => s.Attendances)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(a => a.MarkedByUser)
                .WithMany()
                .HasForeignKey(a => a.MarkedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
