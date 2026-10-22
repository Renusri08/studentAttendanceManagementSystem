# Student Attendance Management

A server-rendered ASP.NET Core MVC application for managing student attendance. Users can create an account, sign in, filter active students by department and year, and record each student's attendance.

## Prerequisites

- .NET 10 SDK
- SQL Server 2022 or later, or an Azure SQL Database
- A SQL Server account/database permission that can create a database and apply schema changes
- (Optional) SQL Server Management Studio (SSMS) or Azure Data Studio to inspect and populate the database

Check that the .NET SDK is available:

```bash
dotnet --version
```

## Database configuration

The application uses Entity Framework Core with the SQL Server provider. It reads the connection string named `DefaultConnection`. Keep credentials out of source control; for local development, use .NET Secret Manager from the project directory:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=StudentAttendanceManagement;User Id=YOUR_SQL_LOGIN;Encrypt=True;TrustServerCertificate=True;"
```

Replace the server and login values with those for your SQL Server. For SQL Server authentication, also add `Password=` and your SQL login password to the connection string locally; Secret Manager keeps this value out of source control. For a SQL Server instance using Windows authentication, a typical connection string is:

```text
Server=localhost;Database=StudentAttendanceManagement;Trusted_Connection=True;TrustServerCertificate=True;
```

The application does not include a default connection string. Configure `DefaultConnection` before starting the application. For deployed environments, set it with the hosting platform's secret/configuration service or the `ConnectionStrings__DefaultConnection` environment variable.

## Create or update the database

Restore the project dependencies:

```bash
dotnet restore
```

Install the Entity Framework CLI tool if it is not already installed:

```bash
dotnet tool install --global dotnet-ef --version 10.0.13
```

Apply the existing migrations to create or update the database:

```bash
dotnet ef database update
```

The initial migration creates the `Users`, `Departments`, `Students`, and `Attendance` tables and their constraints. It does not add departments or students. To use the attendance workflow, add active department and student records to the database using SSMS, Azure Data Studio, or another approved data-loading process. A department and a student must exist before they can be selected in the app.

For a disposable local database, this optional example adds one department and two sample students. Run it once in the `StudentAttendanceManagement` database and replace the sample details as needed:

```sql
IF NOT EXISTS (
    SELECT 1 FROM dbo.Departments
    WHERE DepartmentName = N'Computer Science'
)
BEGIN
    INSERT INTO dbo.Departments (DepartmentName, IsActive)
    VALUES (N'Computer Science', 1);
END;

DECLARE @DepartmentId int = (
    SELECT DepartmentId
    FROM dbo.Departments
    WHERE DepartmentName = N'Computer Science'
);

IF NOT EXISTS (SELECT 1 FROM dbo.Students WHERE RollNumber = N'CS-001')
BEGIN
    INSERT INTO dbo.Students
        (RollNumber, StudentName, DepartmentId, YearOfStudy, IsActive)
    VALUES
        (N'CS-001', N'Alex Morgan', @DepartmentId, 1, 1);
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Students WHERE RollNumber = N'CS-002')
BEGIN
    INSERT INTO dbo.Students
        (RollNumber, StudentName, DepartmentId, YearOfStudy, IsActive)
    VALUES
        (N'CS-002', N'Jordan Lee', @DepartmentId, 1, 1);
END;
```

## Run the application

From the project directory:

```bash
dotnet run --launch-profile http
```

Open [http://localhost:5171](http://localhost:5171). The HTTPS launch profile is also available:

```bash
dotnet run --launch-profile https
```

It uses `https://localhost:7164` and `http://localhost:5171`. If the local HTTPS certificate is not trusted, configure a development certificate or use the HTTP profile for local development.

## First-time use

1. Open the application and create an account on the sign-up page.
2. Sign in with the username or email address and PIN used at sign-up.
3. Open **Attendance Management**.
4. Select a department and year, then choose a student.
5. Select **Present** or **Absent** and submit attendance.

Attendance is recorded for the current date. The database enforces one attendance record per student per day.

## Project structure

- `Controllers/` — account, dashboard, and attendance request handling
- `Data/AppDbContext.cs` — Entity Framework Core database model and constraints
- `Migrations/` — database schema migrations
- `Models/` — database entities
- `ViewModels/` — form and page models with validation rules
- `Views/` — MVC Razor pages
- `wwwroot/` — CSS, JavaScript, and static assets

## Configuration notes

- `appsettings.json` contains general logging settings. SQL command information logs are suppressed at the `Warning` level.
- `appsettings.Development.json` is ignored by Git and can hold local-only settings; do not commit credentials.
- Account PINs are stored as password hashes, not as plain text.
