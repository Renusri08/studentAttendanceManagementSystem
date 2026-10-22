# Student Attendance Management

A server-rendered ASP.NET Core MVC application for managing student attendance. Users can create an account, sign in, filter active students by department and year, and record attendance as Present or Absent.

## Prerequisites

- .NET 10 SDK
- SQL Server 2022 or later, or an Azure SQL Database
- SQL Server database permissions to create a database and apply migrations

### Development Environment

**Windows:**
- Visual Studio Community
- Select the **ASP.NET and web development** workload during installation.
- (Optional) SQL Server Management Studio (SSMS) to manage the database.

**Ubuntu/Linux:**
- Visual Studio Code
- Install the **C# Dev Kit** extension.
- Install the **SQL Server (mssql)** extension to execute SQL queries.
- SQL Server and the `sqlcmd` command-line tool.

Check that the .NET SDK is available:

```bash
dotnet --version
```

## Database Configuration

The application uses Entity Framework Core with the SQL Server provider. It reads the connection string named `DefaultConnection`.

For local development, run the following command from the project directory:

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=StudentAttendanceManagement;User Id=YOUR_SQL_LOGIN;Password=YOUR_SQL_PASSWORD;Encrypt=True;TrustServerCertificate=True;"
```

Replace the login and password with your SQL Server credentials. If User Secrets has already been initialized, skip the first command.

Do not commit database credentials to GitHub.

## Create or Update the Database

Restore the project dependencies:

```bash
dotnet restore
```

Install the Entity Framework CLI tool if it is not already installed:

```bash
dotnet tool install --global dotnet-ef --version 10.0.13
```

Apply the existing migrations:

```bash
dotnet ef database update
```

This creates or updates the database tables: `Users`, `Departments`, `Students`, and `Attendance`.

## Insert Dummy Data

After applying the migrations, connect to the `StudentAttendanceManagement` database and execute the following SQL script.

```sql
INSERT INTO Departments (DepartmentName, IsActive)
VALUES ('CSE', 1), ('ECE', 1), ('EEE', 1), ('MECH', 1), ('CIVIL', 1), ('MME', 1);

INSERT INTO Students
    (RollNumber, StudentName, DepartmentId, YearOfStudy, IsActive)
VALUES
    ('CSE001', 'Arun Kumar', 7, 1, 1),
    ('CSE002', 'Priya',     7, 2, 1),
    ('CSE003', 'Rahul',     7, 2, 1),
    ('ECE001', 'Sneha',     8, 1, 1),
    ('ECE002', 'Kiran',     8, 4, 1);
```

**Note:** This script assumes that CSE has `DepartmentId = 7` and ECE has `DepartmentId = 8`. Check the actual IDs in your database before running the script. If the IDs are different, update them accordingly.

## Run the Application

From the project directory, run:

```bash
dotnet run --launch-profile http
```

Open [http://localhost:5171](http://localhost:5171) in your browser.

The HTTPS launch profile is also available:

```bash
dotnet run --launch-profile https
```

HTTPS URL: [https://localhost:7164](https://localhost:7164)

## Application Flow

1. **Sign Up:** Create an account using the required details and a PIN.
2. **Login:** Sign in using the username or email address and PIN.
3. **Attendance Management:** Open the attendance page.
4. **Filter Students:** Select a department and year to display the corresponding active students.
5. **Mark Attendance:** Select Present or Absent for each student.
6. **Submit Attendance:** Save the attendance records to the database.

For example, selecting the CSE department and second year should display Priya and Rahul from the dummy data.

Attendance is recorded for the current date. The database prevents duplicate attendance records for the same student on the same day.

## Project Structure

- `Controllers/` — Handles application requests.
- `Data/AppDbContext.cs` — Configures the database and relationships.
- `Migrations/` — Contains database schema changes.
- `Models/` — Contains database entity classes.
- `ViewModels/` — Contains form models and validation rules.
- `Views/` — Contains Razor views.
- `wwwroot/` — Contains CSS, JavaScript, and other static files.

## Technologies Used

- C#
- ASP.NET Core MVC
- .NET 10
- Entity Framework Core
- SQL Server
- Razor Views
- HTML, CSS, and JavaScript
- Cookie-based authentication
- Git and GitHub

## Configuration Notes

- `appsettings.json` contains general application settings and logging configuration.
- `appsettings.Development.json` is ignored by Git and can contain local development settings. Do not commit credentials.
- Entity Framework Core migrations are used to create and update the database schema.
