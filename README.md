# Sha8alny - Student-Company Project Matching Platform

> **Sha8alny** (شغلني) means "Employ Me" in Arabic - A platform connecting students with real-world project opportunities from companies.

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-9.0-512BD4)](https://asp.net/)
[![Entity Framework Core](https://img.shields.io/badge/EF_Core-9.0-512BD4)](https://docs.microsoft.com/en-us/ef/core/)
[![SQL Server](https://img.shields.io/badge/SQL_Server-2019+-CC2927)](https://www.microsoft.com/sql-server)

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Architecture](#architecture)
- [Tech Stack](#tech-stack)
- [Getting Started](#getting-started)
- [Project Structure](#project-structure)
- [API Documentation](#api-documentation)
- [Configuration](#configuration)
- [Database](#database)
- [Authentication](#authentication)
- [Contributing](#contributing)

---

## 🎯 Overview

Sha8alny is a comprehensive platform that bridges the gap between university students and companies by providing a marketplace for real-world project opportunities. Students can browse projects, apply with their skills, and gain practical experience, while companies can find talented students for their projects.

### Key Objectives

- **For Students**: Gain practical experience, build portfolio, network with companies
- **For Companies**: Access talented students, complete projects cost-effectively, identify potential hires
- **For Education**: Bridge academia-industry gap, enhance student employability

---

## ✨ Features

### Student Features
- ✅ Profile management with skills, university info, and academic details
- ✅ Browse and search project opportunities
- ✅ Apply for projects with cover letters
- ✅ Track application status
- ✅ Skill-based matching

### Company Features
- ✅ Company profile with verification system
- ✅ Post projects/opportunities (Internships, Freelance, Part-time, Full-time)
- ✅ Review student applications
- ✅ Accept/reject applications with feedback
- ✅ Project management (active, paused, completed)

### Platform Features
- ✅ JWT-based authentication
- ✅ Role-based authorization (Student, Company, Admin)
- ✅ Email notifications
- ✅ RESTful API
- ✅ Swagger API documentation
- ✅ Advanced search and filtering

---

## 🏗️ Architecture

Sha8alny follows **Onion Architecture** (Clean Architecture) principles, ensuring separation of concerns, testability, and maintainability.

```
┌─────────────────────────────────────────────────────────┐
│                    Presentation Layer                    │
│              (API Controllers, Swagger UI)               │
└─────────────────────┬───────────────────────────────────┘
                      │
┌─────────────────────▼───────────────────────────────────┐
│              Infrastructure Layer                        │
│     (Services: Token, Email, CurrentUser, DbContext)    │
└─────────────────────┬───────────────────────────────────┘
                      │
┌─────────────────────▼───────────────────────────────────┐
│              Application Layer                           │
│   (Business Logic, Use Cases, DTOs, Interfaces)         │
└─────────────────────┬───────────────────────────────────┘
                      │
┌─────────────────────▼───────────────────────────────────┐
│                 Domain Layer                             │
│        (Entities, Value Objects, Enums)                 │
└─────────────────────────────────────────────────────────┘
```

### Layer Responsibilities

| Layer | Responsibility | Dependencies |
|-------|----------------|--------------|
| **Domain** | Business entities, value objects, enums | None |
| **Application** | Business logic, use cases, DTOs, interfaces | Domain |
| **Infrastructure** | Data access, external services | Application, Domain |
| **Presentation** | API controllers, request/response handling | Application |

---

## 🛠️ Tech Stack

### Backend
- **.NET 9.0** - Modern cross-platform framework
- **ASP.NET Core** - Web API framework
- **Entity Framework Core 9.0** - ORM with SQL Server
- **JWT Bearer Authentication** - Secure token-based auth
- **BCrypt.Net** - Password hashing
- **Swashbuckle (Swagger)** - API documentation

### Database
- **SQL Server** - Relational database
- **EF Core Migrations** - Database versioning

### Tools & Patterns
- **Repository Pattern** - Data access abstraction
- **Unit of Work Pattern** - Transaction management
- **Dependency Injection** - Loose coupling
- **DTOs** - Data transfer objects
- **API Response Wrapper** - Consistent response format

---

## 🚀 Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server 2019+](https://www.microsoft.com/sql-server) (or SQL Server Express)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/ninjam5/Sha8alny.git
   cd Sha8alny
   ```

2. **Update connection string**
   
   Edit `Sh8lny.Web/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=Sha8alnyDb;Trusted_Connection=true;TrustServerCertificate=true;"
     }
   }
   ```

3. **Apply database migrations**
   ```bash
   cd Sh8lny.Web
   dotnet ef database update --project ../Infrastructure/Sh8lny.Persistence
   ```

4. **Build the solution**
   ```bash
   dotnet build Sh8lnySolution.sln
   ```

5. **Run the application**
   ```bash
   dotnet run --project Sh8lny.Web
   ```

6. **Access Swagger UI**
   
   Open your browser and navigate to: `https://localhost:5001`

---

## 📁 Project Structure

```
Sha8alny/
├── Core/
│   ├── Sh8lny.Domain/              # Domain entities, value objects, enums
│   │   ├── Entities/
│   │   │   ├── Student.cs
│   │   │   ├── Company.cs
│   │   │   ├── Project.cs
│   │   │   ├── Application.cs
│   │   │   └── Skill.cs
│   │   ├── Enums/
│   │   │   ├── ProjectType.cs
│   │   │   ├── ProjectStatus.cs
│   │   │   └── ApplicationStatus.cs
│   │   ├── Interfaces/
│   │   └── ValueObjects/
│   │
│   └── Sh8lny.Application/         # Business logic layer
│       ├── DTOs/                   # Data Transfer Objects
│       ├── Interfaces/             # Service interfaces
│       ├── UseCases/               # Service implementations
│       │   ├── Auth/
│       │   ├── Students/
│       │   ├── Companies/
│       │   ├── Projects/
│       │   └── Applications/
│       └── Extensions/             # DI extensions
│
├── Infrastructure/
│   └── Sh8lny.Persistence/         # Data access layer
│       ├── Contexts/               # DbContext
│       ├── Repositories/           # Repository implementations
│       ├── Configurations/         # Entity configurations
│       ├── Migrations/             # EF migrations
│       └── Extensions/             # DI extensions
│
├── Sh8lny.Web/                     # API presentation layer
│   ├── Controllers/                # API controllers
│   │   ├── AuthController.cs
│   │   ├── StudentsController.cs
│   │   ├── CompaniesController.cs
│   │   ├── ProjectsController.cs
│   │   └── ApplicationsController.cs
│   ├── Services/                   # Infrastructure services
│   │   ├── TokenService.cs
│   │   ├── EmailService.cs
│   │   └── CurrentUserService.cs
│   ├── Extensions/                 # DI extensions
│   ├── Program.cs                  # Application entry point
│   └── appsettings.json            # Configuration
│
└── Sh8lny.Shared/                  # Shared utilities
```

---

## 📚 API Documentation

The API is fully documented with Swagger. Access the interactive documentation at:

**Swagger UI**: `https://localhost:5001` (when running locally)

### API Endpoints Overview

#### Authentication (`/api/auth`)
- `POST /api/auth/register/student` - Register student account
- `POST /api/auth/register/company` - Register company account
- `POST /api/auth/login` - Login
- `POST /api/auth/refresh` - Refresh access token
- `POST /api/auth/verify-email` - Verify email
- `POST /api/auth/forgot-password` - Request password reset
- `POST /api/auth/reset-password` - Reset password
- `POST /api/auth/change-password` - Change password

#### Students (`/api/students`)
- `GET /api/students` - Get all students (paginated)
- `GET /api/students/{id}` - Get student by ID
- `POST /api/students` - Create student profile
- `PUT /api/students/{id}` - Update student profile
- `DELETE /api/students/{id}` - Delete student profile
- `POST /api/students/{studentId}/skills/{skillId}` - Add skill
- `DELETE /api/students/{studentId}/skills/{skillId}` - Remove skill

#### Companies (`/api/companies`)
- `GET /api/companies` - Get all companies (paginated)
- `GET /api/companies/{id}` - Get company by ID
- `POST /api/companies` - Create company profile
- `PUT /api/companies/{id}` - Update company profile
- `DELETE /api/companies/{id}` - Delete company profile
- `POST /api/companies/{id}/verify` - Verify company (Admin only)

#### Projects (`/api/projects`)
- `GET /api/projects` - Get all projects (paginated)
- `GET /api/projects/{id}` - Get project by ID
- `GET /api/projects/active` - Get active projects
- `GET /api/projects/company/{companyId}` - Get company projects
- `GET /api/projects/search?searchTerm={term}` - Search projects
- `POST /api/projects` - Create project
- `PUT /api/projects/{id}` - Update project
- `DELETE /api/projects/{id}` - Delete project
- `PATCH /api/projects/{id}/status` - Update project status

#### Applications (`/api/applications`)
- `GET /api/applications` - Get all applications (paginated)
- `GET /api/applications/{id}` - Get application by ID
- `GET /api/applications/project/{projectId}` - Get project applications
- `GET /api/applications/student/{studentId}` - Get student applications
- `POST /api/applications` - Submit application
- `PUT /api/applications/{id}/review` - Review application
- `DELETE /api/applications/{id}/withdraw` - Withdraw application
- `GET /api/applications/check?projectId={id}&studentId={id}` - Check if applied

---

## ⚙️ Configuration

### App Settings (`appsettings.json`)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=Sha8alnyDb;Trusted_Connection=true;TrustServerCertificate=true;"
  },
  "Jwt": {
    "Key": "ThisIsAVerySecretKeyForJwtTokenGenerationAndValidation123456",
    "Issuer": "Sha8alny",
    "Audience": "Sha8alnyUsers"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Environment Variables

For production, use environment variables instead of hardcoding sensitive data:

```bash
export ConnectionStrings__DefaultConnection="Server=...;Database=...;"
export Jwt__Key="your-secret-key-here"
```

---

## 🗄️ Database

### Entities

- **Student**: University students seeking opportunities
- **Company**: Organizations posting projects
- **Project**: Work opportunities (internships, freelance, jobs)
- **Application**: Student applications to projects
- **Skill**: Technical and soft skills
- **StudentSkill**: Many-to-many relationship

### Entity Relationships

```
Student 1──N Application N──1 Project N──1 Company
   │
   └── N StudentSkill N──1 Skill
```

### Migrations

```bash
# Add new migration
dotnet ef migrations add MigrationName --project Infrastructure/Sh8lny.Persistence --startup-project Sh8lny.Web

# Update database
dotnet ef database update --project Infrastructure/Sh8lny.Persistence --startup-project Sh8lny.Web

# Remove last migration
dotnet ef migrations remove --project Infrastructure/Sh8lny.Persistence --startup-project Sh8lny.Web
```

---

## 🔐 Authentication

### JWT Bearer Authentication

The API uses JWT (JSON Web Tokens) for secure authentication.

#### Token Structure

- **Access Token**: Valid for 1 hour
- **Refresh Token**: Random 64-byte Base64 string
- **Claims**: userId, email, role (Student/Company/Admin)

#### Using Authentication in Swagger

1. Click **Authorize** button
2. Enter: `Bearer {your_access_token}`
3. Click **Authorize**

#### Example Login Flow

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "student@example.com",
  "password": "Password123!"
}
```

**Response:**
```json
{
  "succeeded": true,
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIs...",
    "refreshToken": "abc123...",
    "expiresAt": "2025-11-14T12:00:00Z",
    "userType": "Student"
  }
}
```

### Password Requirements

- Minimum 8 characters
- At least one uppercase letter
- At least one lowercase letter
- At least one number
- At least one special character

---

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

### Code Style

- Follow [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Use meaningful variable and method names
- Add XML documentation comments for public APIs
- Write unit tests for new features

---

## 📊 Project Statistics

- **42 API Endpoints** across 5 controllers
- **5 Application Services** (1,811 lines of business logic)
- **3 Infrastructure Services** (Token, Email, CurrentUser)
- **5 Domain Entities** with relationships
- **3 Enum Types** for type safety
- **100% JWT Secured** endpoints
- **Onion Architecture** for clean code

---

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 👥 Team

**Developer**: [ninjam5](https://github.com/ninjam5)

**Project**: Sha8alny - Student-Company Project Matching Platform

**Repository**: [github.com/ninjam5/Sha8alny](https://github.com/ninjam5/Sha8alny)

---

## 📞 Support

For issues, questions, or contributions:
- **GitHub Issues**: [Create an issue](https://github.com/ninjam5/Sha8alny/issues)
- **Email**: support@sha8alny.com

---

<div align="center">

**Built with ❤️ using .NET 9.0 and Clean Architecture**

⭐ Star this repo if you find it helpful!

</div>
