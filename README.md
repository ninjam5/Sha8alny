🚀 Sha8alny (شغلني)A robust .NET 9 Web API Backend for a Freelancing Marketplace connecting Students with Companies.Sha8alny is a comprehensive freelancing platform that bridges the gap between talented students and companies seeking skilled professionals. Built with modern technologies and best practices, it provides a complete solution for job posting, application management, real-time communication, and payment processing.✨ Key Features🔐 Role-Based AuthenticationAdmin - Complete system control with "God Mode" capabilitiesCompany - Post jobs, manage applications, and hire studentsStudent - Browse opportunities, apply for jobs, and build portfolio📋 Complete Job WorkflowPost Job → Apply → Milestones → Complete → Payment → Review
Companies post detailed job opportunities with required skillsStudents apply with their profiles and portfoliosTrack progress through milestone-based project managementSecure payment processing upon completionMutual review system for quality assurance💬 Real-Time CommunicationSignalR-powered Chat - Instant messaging between students and companiesLive Notifications - Real-time updates for applications, messages, and project statusPersistent conversation history🔍 Smart Search & DiscoveryAdvanced filtering by skills, location, and project typePagination support for optimized performanceBookmark/save opportunities for later review🎓 Certificate GenerationAutomatic certificate generation upon project completionProfessional templates for student portfoliosVerifiable credentials👨‍💼 Admin DashboardComplete system oversight and managementUser management and moderationAnalytics and reportingPlatform configuration🏗️ ArchitectureSha8alny follows Onion Architecture (Clean Architecture) principles, ensuring separation of concerns, testability, and maintainability.graph TD
    %% Styling
    classDef core fill:#f9f,stroke:#333,stroke-width:2px;
    classDef service fill:#bbf,stroke:#333,stroke-width:2px;
    classDef infra fill:#bfb,stroke:#333,stroke-width:2px;
    classDef web fill:#fbb,stroke:#333,stroke-width:2px;

    subgraph User_Interaction ["User Interaction"]
        Client[("Frontend / Postman")]
    end

    subgraph Web_Layer ["Web Layer (Presentation)"]
        Controllers["API Controllers"]
        SignalR["SignalR Hubs"]
        Middleware["Global Error Handling"]
    end

    subgraph Service_Layer ["Service Layer (Business Logic)"]
        Services["Business Services<br>(Auth, Project, Chat)"]
        Validators["FluentValidation"]
    end

    subgraph Core_Layer ["Core Layer (The Heart)"]
        Interfaces["Abstractions<br>(IRepository, IService)"]
        Domain["Domain Entities<br>(User, Project, Review)"]
    end

    subgraph Infra_Layer ["Infrastructure Layer"]
        DbContext["EF Core DbContext"]
        Repos["Repository Implementation"]
        Migrations["SQL Migrations"]
        Seeder["Data Seeder"]
    end

    subgraph Database ["Persistence"]
        SQL[(SQL Server 2022)]
    end

    %% Relationships
    Client ==> Controllers
    Client ==> SignalR

    Controllers --> Services
    SignalR --> Services

    Services --> Interfaces
    Services --> Domain

    Repos -.->|Implements| Interfaces
    Repos --> DbContext
    DbContext --> Domain

    Web_Layer -.->|Dependency Injection| Infra_Layer

    DbContext ==> SQL

    %% Apply Styles
    class Interfaces,Domain core;
    class Services,Validators service;
    class DbContext,Repos,Migrations,Seeder infra;
    class Controllers,SignalR,Middleware web;
Core Layer (Business Logic - No Dependencies)Sh8lny.Domain: Pure business entities (User, Project, Application, Message, etc.)Sh8lny.Abstraction: Interfaces and contracts (IGenericRepository, IUnitOfWork)Sh8lny.Service: Business logic and service implementationsInfrastructure Layer (External Concerns)Sh8lny.Persistence: Entity Framework Core, DbContext, Repository implementationsSh8lny.Presentation: Cross-cutting concerns and shared presentation logicWeb Layer (Entry Point)Sh8lny.Web: ASP.NET Core Web API, Controllers, DTOs, SignalR Hubs, MiddlewareBenefits:✅ Testable and maintainable✅ Database-agnostic core✅ Easy to swap infrastructure components✅ Clear dependency flow (inward only)🗄️ Database SchemaThe following ER Diagram illustrates the core entity relationships within the SQL Server database.erDiagram
    User ||--o| Student : "is a"
    User ||--o| Company : "is a"
    User ||--o{ UserSettings : "has"
    User ||--o{ Message : "sends/receives"

    Company ||--o{ Project : "posts"
    
    Project ||--o{ ProjectModule : "defines milestones"
    Project ||--o{ Application : "receives"
    Project ||--o{ Skill : "requires"

    Student ||--o{ StudentSkill : "has"
    Student ||--o{ Application : "applies for"
    
    Application ||--o{ ModuleProgress : "tracks progress"
    Application ||--o| Certificate : "generates upon completion"
    Application ||--o| Transaction : "payment record"

    Application ||--o{ Review : "results in"
    
    Conversation ||--o{ Message : "contains"
    Conversation }|--|{ User : "participants"
📋 PrerequisitesThat's it! Just one requirement:Docker Desktop (Download here)Docker handles everything: .NET SDK, SQL Server, dependencies, and configuration. No manual setup needed! 🎉🚀 Getting Started (The "Magic" Way)1️⃣ Clone the Repositorygit clone [https://github.com/ninjam5/Sha8alny.git](https://github.com/ninjam5/Sha8alny.git)
cd Sha8alny
2️⃣ Run with Docker Composedocker-compose up --build
3️⃣ Wait for the Magic ✨The application will:🐳 Build the .NET 9 API container🗄️ Spin up SQL Server 2022🔄 Automatically run database migrations🌱 Seed demo data (users, skills, universities, etc.)🚀 Start the API server4️⃣ Access the ApplicationOnce you see Now listening on: http://[::]:8080, open:Swagger UI: http://localhost:5000/swaggerAPI Base URL: http://localhost:5000Note: The first run may take 2-3 minutes as Docker downloads images and builds the project.🔑 Default Demo CredentialsUse these pre-seeded accounts to explore the platform:RoleEmailPasswordDescription👨‍💼 Adminadmin@sha8alny.comPassword123!Full system access and control🏢 Companytechcorp@test.comPassword123!Post jobs and hire students🎓 Studentstudent@test.comPassword123!Apply for jobs and complete projectsSecurity Note: Change these credentials before deploying to production!📖 API DocumentationComprehensive API documentation is available via Swagger UI:🔗 http://localhost:5000/swaggerAvailable Endpoints:🔐 Authentication - Login, Register, Refresh Tokens👤 User Management - Profiles, Skills, Education📋 Projects - CRUD operations, Search, Filter📝 Applications - Apply, Accept, Reject, Track Progress💬 Chat - Real-time messaging via SignalR🔔 Notifications - Real-time updates💳 Payments - Process payments and transactions⭐ Reviews - Rate and review completed projects👨‍💼 Admin - System management and analyticsTip: Use the "Authorize" button in Swagger UI to test authenticated endpoints.📁 Project StructureSha8alny/
├── 📂 Core/                          # Business Logic Layer (No External Dependencies)
│   ├── Sh8lny.Domain/                # Entities (User, Project, Application, etc.)
│   ├── Sh8lny.Abstraction/           # Interfaces (IRepository, IUnitOfWork)
│   └── Sh8lny.Service/               # Business Services
│
├── 📂 Infrastructure/                # External Concerns Layer
│   ├── Sh8lny.Persistence/           # EF Core, DbContext, Repositories
│   └── Sh8lny.Presentation/          # Shared Presentation Logic
│
├── 📂 Sh8lny.Web/                    # API Entry Point
│   ├── Controllers/                  # REST API Endpoints
│   ├── DTOs/                         # Data Transfer Objects
│   ├── Hubs/                         # SignalR Hubs (Chat, Notifications)
│   ├── Mappings/                     # AutoMapper Profiles
│   ├── Services/                     # Web-specific Services
│   └── Program.cs                    # Application Configuration
│
├── 📂 Sh8lny.Shared/                 # Shared Utilities
│
├── 🐳 docker-compose.yml             # Container Orchestration
├── 🐳 Dockerfile                     # API Container Definition
└── 📄 Sh8lnySolution.sln             # Visual Studio Solution
🛠️ Technology StackCategoryTechnologyFrameworkASP.NET Core 9 Web APILanguageC# 13DatabaseSQL Server 2022ORMEntity Framework Core 9AuthenticationJWT (Access + Refresh Tokens)Password HashingBCryptReal-timeSignalRContainerizationDocker & Docker ComposeAPI DocsSwagger/OpenAPIMappingAutoMapperPatternsRepository, Unit of Work, DI🔧 Advanced ConfigurationEnvironment VariablesYou can customize the deployment by modifying docker-compose.yml:environment:
  - ASPNETCORE_ENVIRONMENT=Development
  - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=Sh8lnyDB;...
Database ConnectionThe connection string is automatically configured in Docker Compose. For local development without Docker:Update appsettings.json:"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=Sh8lnyDB;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"
}
Run migrations:dotnet ef database update --project Infrastructure/Sh8lny.Persistence --startup-project Sh8lny.Web
🧪 DevelopmentBuild the Solutiondotnet build
Run Locally (without Docker)cd Sh8lny.Web
dotnet run
Create Migrationdotnet ef migrations add MigrationName --project Infrastructure/Sh8lny.Persistence --startup-project Sh8lny.Web
🤝 ContributingContributions are welcome! Please feel free to submit a Pull Request.Fork the repositoryCreate your feature branch (git checkout -b feature/AmazingFeature)Commit your changes (git commit -m 'Add some AmazingFeature')Push to the branch (git push origin feature/AmazingFeature)Open a Pull Request📄 LicenseThis project is licensed under the MIT License - see the LICENSE file for details.👥 AuthorsSha8alny Team - Initial work🙏 AcknowledgmentsBuilt with ❤️ using .NET 9Inspired by modern freelancing platformsSpecial thanks to the .NET community<div align="center"><p>Made with ❤️ in Egypt</p><p>⭐ Star this repo if you find it helpful!</p></div>
