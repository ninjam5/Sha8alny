# Sha8lny Platform - .NET Backend

## 📋 Overview

Complete **ASP.NET Core 8.0 + Entity Framework Core + MSSQL** backend implementation for the **Sha8lny** platform - a student training and internship management system connecting students, companies, and universities.

**Generated from Figma Analysis:** `hIXl1JjbjmoFIhDDaCODyF`

---

## 🏗️ Architecture

### Technology Stack
- **Framework:** ASP.NET Core 8.0 (Web API)
- **ORM:** Entity Framework Core 8.0
- **Database:** Microsoft SQL Server
- **Authentication:** JWT Bearer Tokens
- **Password Hashing:** BCrypt
- **Documentation:** Swagger/OpenAPI

### Project Structure
```
.NET_Backend/
├── Models/                    # Entity models (20 entities)
│   ├── User.cs
│   ├── Student.cs
│   ├── Company.cs
│   ├── University.cs
│   ├── Department.cs
│   ├── Skill.cs
│   ├── StudentSkill.cs
│   ├── Project.cs
│   ├── ProjectRequiredSkill.cs
│   ├── Application.cs
│   ├── ProjectGroup.cs
│   ├── GroupMember.cs
│   ├── Conversation.cs
│   ├── ConversationParticipant.cs
│   ├── Message.cs
│   ├── Certificate.cs
│   ├── Notification.cs
│   ├── ActivityLog.cs
│   ├── DashboardMetric.cs
│   └── UserSettings.cs
│
├── Data/                      # Database context & configurations
│   ├── Sha8lnyDbContext.cs
│   └── Configurations/        # Fluent API configurations
│       ├── UserConfiguration.cs
│       ├── StudentConfiguration.cs
│       ├── CompanyConfiguration.cs
│       └── ... (17 more)
│
├── Controllers/               # API endpoints (to be created)
├── Services/                  # Business logic (to be created)
├── DTOs/                      # Data transfer objects (to be created)
├── Repositories/              # Data access layer (to be created)
├── Middleware/                # Custom middleware (to be created)
├── Migrations/                # EF Core migrations (auto-generated)
│
├── Program.cs                 # Application entry point
├── appsettings.json          # Configuration
└── Sha8lny.csproj            # Project file
```

---

## 🚀 Quick Start

### Prerequisites
- **.NET 8.0 SDK** or later
- **SQL Server 2019+** or **Azure SQL Database**
- **Visual Studio 2022** or **VS Code** with C# extension

### 1. Install Dependencies

```bash
cd .NET_Backend
dotnet restore
```

### 2. Configure Database Connection

Edit `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=Sha8lnyDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

**For Azure SQL:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:YOUR_SERVER.database.windows.net,1433;Initial Catalog=Sha8lnyDb;User ID=admin;Password=YOUR_PASSWORD;Encrypt=True;"
  }
}
```

### 3. Create Initial Migration

```bash
dotnet ef migrations add InitialCreate
```

### 4. Apply Migration (Create Database)

```bash
dotnet ef database update
```

### 5. Run the Application

```bash
dotnet run
```

API will be available at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `https://localhost:5001/swagger`

---

## 📊 Database Schema

### Core Tables (20)

#### **Authentication & Users**
- `Users` - Central authentication (Email, PasswordHash, UserType)
- `UserSettings` - User preferences and privacy settings

#### **Profiles**
- `Students` - Student profiles with academic info
- `Companies` - Company profiles with partnership details
- `Universities` - University information
- `Departments` - University departments (Computer, Electric, etc.)

#### **Skills System**
- `Skills` - Master skills table (Backend, Frontend, UI/UX, etc.)
- `StudentSkills` - Student-Skill junction with proficiency levels
- `ProjectRequiredSkills` - Project-Skill junction

#### **Projects & Applications**
- `Projects` - Training opportunities/internships
- `Applications` - Student applications to projects
  - Status: Submit → Pending → Under Review → Accepted/Rejected
  - TimePreference: AM | PM | Flexible

#### **Collaboration**
- `ProjectGroups` - Team groups for projects
- `GroupMembers` - Group membership tracking

#### **Messaging**
- `Conversations` - Direct, Group, or Project conversations
- `ConversationParticipants` - Conversation membership
- `Messages` - Chat messages with attachments

#### **Analytics & Achievements**
- `Certificates` - Student completion certificates
- `Notifications` - User notifications system
- `ActivityLog` - Audit trail for all actions
- `DashboardMetrics` - Cached analytics
  - "3 new applicants"
  - "2 projects near deadline"
  - "15% increase in activity"

---

## 🔧 Entity Framework Commands

### Create a New Migration
```bash
dotnet ef migrations add <MigrationName>
```

### Apply Migrations
```bash
dotnet ef database update
```

### Rollback to Specific Migration
```bash
dotnet ef database update <MigrationName>
```

### Remove Last Migration (if not applied)
```bash
dotnet ef migrations remove
```

### Generate SQL Script
```bash
dotnet ef migrations script
```

### Drop Database
```bash
dotnet ef database drop
```

---

## 📝 Models Overview

### User (Authentication)
```csharp
public class User
{
    public int UserID { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public UserType UserType { get; set; } // Student, Company, University, Admin
    public bool IsEmailVerified { get; set; }
    public string? VerificationCode { get; set; }
    // Navigation properties...
}
```

### Student
```csharp
public class Student
{
    public int StudentID { get; set; }
    public int UserID { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int? UniversityID { get; set; }
    public int? DepartmentID { get; set; }
    public AcademicYear? AcademicYear { get; set; } // 1st, 2nd, 3rd, 4th Year
    public StudentStatus Status { get; set; }
    public int ProfileCompleteness { get; set; }
    // Navigation properties...
}
```

### Project
```csharp
public class Project
{
    public int ProjectID { get; set; }
    public int CompanyID { get; set; }
    public string ProjectName { get; set; }
    public string Description { get; set; }
    public ProjectType? ProjectType { get; set; } // Internship, Training, etc.
    public DateTime Deadline { get; set; }
    public ProjectStatus Status { get; set; }
    public string? CreatedByName { get; set; } // "Jane Smith", "Alan Cain"
    public int ApplicationCount { get; set; }
    // Navigation properties...
}
```

### Application
```csharp
public class Application
{
    public int ApplicationID { get; set; }
    public int ProjectID { get; set; }
    public int StudentID { get; set; }
    public string? CoverLetter { get; set; }
    public ApplicationStatus Status { get; set; } // Submit, Pending, Accepted, etc.
    public TimePreference? TimePreference { get; set; } // AM, PM, Flexible
    public DateTime AppliedAt { get; set; }
    // Navigation properties...
}
```

---

## 🔐 Security Features

### Password Hashing
Uses **BCrypt** for secure password storage:
```csharp
var passwordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword);
bool isValid = BCrypt.Net.BCrypt.Verify(plainPassword, passwordHash);
```

### Email Verification
- `VerificationCode` stored in `Users` table
- Time-limited with `VerificationCodeExpiry`
- Verified via `IsEmailVerified` flag

### Activity Logging
All sensitive operations logged in `ActivityLog`:
- User login/logout
- Project creation
- Application submission
- Status changes

---

## 📦 Seeded Data

Initial database includes:

### Skills (13 pre-seeded)
- Backend
- Frontend
- UI&UX
- Mobile App
- Web Development
- AI
- Big Data
- Testing
- Marketing
- Social Media
- Photoshop
- Cybersecurity
- Flutter

---

## 🎯 Next Steps

### 1. **Create Controllers**
Example: `StudentsController.cs`
```csharp
[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly Sha8lnyDbContext _context;
    
    public StudentsController(Sha8lnyDbContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
    {
        return await _context.Students
            .Include(s => s.University)
            .Include(s => s.Department)
            .Include(s => s.StudentSkills)
            .ThenInclude(ss => ss.Skill)
            .ToListAsync();
    }
}
```

### 2. **Implement Authentication**
- JWT token generation
- Login/Register endpoints
- Password reset flow
- Email verification

### 3. **Create DTOs**
Example: `StudentDto.cs`
```csharp
public class StudentDto
{
    public int StudentID { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string? University { get; set; }
    public string? Department { get; set; }
    public List<string> Skills { get; set; }
}
```

### 4. **Add Business Logic Services**
- `IAuthService` - Authentication logic
- `IStudentService` - Student operations
- `IProjectService` - Project management
- `IApplicationService` - Application workflow
- `INotificationService` - Notification system

### 5. **Implement Repositories**
```csharp
public interface IStudentRepository
{
    Task<Student?> GetByIdAsync(int id);
    Task<IEnumerable<Student>> GetAllAsync();
    Task<Student> CreateAsync(Student student);
    Task UpdateAsync(Student student);
    Task DeleteAsync(int id);
}
```

### 6. **Add AutoMapper**
Map entities to DTOs automatically:
```csharp
CreateMap<Student, StudentDto>()
    .ForMember(dest => dest.FullName, 
               opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));
```

### 7. **Create API Endpoints**

**Students:**
- `GET /api/students` - List all students
- `GET /api/students/{id}` - Get student by ID
- `POST /api/students` - Create student profile
- `PUT /api/students/{id}` - Update student
- `DELETE /api/students/{id}` - Delete student

**Projects:**
- `GET /api/projects` - List active projects
- `GET /api/projects/{id}` - Get project details
- `POST /api/projects` - Create project (Company only)
- `PUT /api/projects/{id}` - Update project
- `GET /api/projects/{id}/applications` - Get project applications

**Applications:**
- `POST /api/applications` - Submit application
- `GET /api/applications/{id}` - Get application details
- `PUT /api/applications/{id}/status` - Update application status
- `GET /api/students/{id}/applications` - Get student's applications

**Dashboard:**
- `GET /api/dashboard/metrics` - Get dashboard statistics
- `GET /api/dashboard/notifications` - Get user notifications

---

## 🧪 Testing

### Unit Tests
```bash
dotnet test
```

### Integration Tests
Test database operations:
```csharp
[Fact]
public async Task CanCreateStudent()
{
    var options = new DbContextOptionsBuilder<Sha8lnyDbContext>()
        .UseInMemoryDatabase(databaseName: "TestDb")
        .Options;

    using var context = new Sha8lnyDbContext(options);
    var student = new Student
    {
        FirstName = "Christine",
        LastName = "Brooks",
        UserID = 1
    };
    
    context.Students.Add(student);
    await context.SaveChangesAsync();
    
    Assert.Equal(1, await context.Students.CountAsync());
}
```

---

## 📚 Related Documentation

- **SQL Schema:** `Sha8lny_ERD_MSSQL.sql`
- **ERD Diagram:** `Sha8lny_ERD_Diagram.md`
- **Figma Analysis:** `FIGMA_PROJECT_ANALYSIS.md`
- **Business Model:** `SHA8LNY_APP_SUMMARY.md`

---

## 🐛 Troubleshooting

### Migration Errors
```bash
# Clear migrations and start fresh
dotnet ef database drop -f
dotnet ef migrations remove
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Connection String Issues
Verify SQL Server is running:
```bash
sqlcmd -S localhost -E -Q "SELECT @@VERSION"
```

### Package Restore Issues
```bash
dotnet clean
dotnet restore --force
dotnet build
```

---

## 📄 License

Generated for the Sha8lny platform based on Figma design analysis.

---

## 👥 Contributors

Generated from Figma project analysis:
- **App Name:** Sha8lny (شغلني - "Employ Me")
- **Target Market:** Egyptian & Middle Eastern universities
- **Features:** Student-Company-University 3-sided marketplace
