# 🚀 Sha8alny (شغلني)



[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)

[![Docker](https://img.shields.io/badge/Docker-Enabled-2496ED?logo=docker)](https://www.docker.com/)

[![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC2927?logo=microsoft-sql-server)](https://www.microsoft.com/sql-server)

[![SignalR](https://img.shields.io/badge/SignalR-Real--time-512BD4)](https://dotnet.microsoft.com/apps/aspnet/signalr)

[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)



> **A robust .NET 9 Web API Backend for a Freelancing Marketplace connecting Students with Companies.**



Sha8alny is a comprehensive freelancing platform that bridges the gap between talented students and companies seeking skilled professionals. Built with modern technologies and best practices, it provides a complete solution for job posting, application management, real-time communication, and payment processing.



---



## ✨ Key Features



### 🔐 **Role-Based Authentication**

- **Admin** - Complete system control with "God Mode" capabilities

- **Company** - Post jobs, manage applications, and hire students

- **Student** - Browse opportunities, apply for jobs, and build portfolio



### 📋 **Complete Job Workflow**

```

Post Job → Apply → Milestones → Complete → Payment → Review

```

- Companies post detailed job opportunities with required skills

- Students apply with their profiles and portfolios

- Track progress through milestone-based project management

- Secure payment processing upon completion

- Mutual review system for quality assurance



### 💬 **Real-Time Communication**

- **SignalR-powered Chat** - Instant messaging between students and companies

- **Live Notifications** - Real-time updates for applications, messages, and project status

- Persistent conversation history



### 🔍 **Smart Search & Discovery**

- Advanced filtering by skills, location, and project type

- Pagination support for optimized performance

- Bookmark/save opportunities for later review



### 🎓 **Certificate Generation**

- Automatic certificate generation upon project completion

- Professional templates for student portfolios

- Verifiable credentials



### 👨‍💼 **Admin Dashboard**

- Complete system oversight and management

- User management and moderation

- Analytics and reporting

- Platform configuration



---



## 🏗️ Architecture



Sha8alny follows **Onion Architecture** (Clean Architecture) principles, ensuring separation of concerns, testability, and maintainability.



```

┌─────────────────────────────────────────────────┐

│              Web Layer (API Entry)              │

│         (Controllers, DTOs, Program.cs)         │

└─────────────────────────────────────────────────┘

                       │

┌─────────────────────────────────────────────────┐

│           Infrastructure Layer                  │

│   (Persistence, External Services, SignalR)     │

└─────────────────────────────────────────────────┘

                       │

┌─────────────────────────────────────────────────┐

│                Core Layer                       │

│    (Domain Entities, Abstractions, Services)    │

└─────────────────────────────────────────────────┘

```



### **Core Layer** (Business Logic - No Dependencies)

- **Sh8lny.Domain**: Pure business entities (User, Project, Application, Message, etc.)

- **Sh8lny.Abstraction**: Interfaces and contracts (IGenericRepository, IUnitOfWork)

- **Sh8lny.Service**: Business logic and service implementations



### **Infrastructure Layer** (External Concerns)

- **Sh8lny.Persistence**: Entity Framework Core, DbContext, Repository implementations

- **Sh8lny.Presentation**: Cross-cutting concerns and shared presentation logic



### **Web Layer** (Entry Point)

- **Sh8lny.Web**: ASP.NET Core Web API, Controllers, DTOs, SignalR Hubs, Middleware



**Benefits:**

- ✅ Testable and maintainable

- ✅ Database-agnostic core

- ✅ Easy to swap infrastructure components

- ✅ Clear dependency flow (inward only)



---



## 📋 Prerequisites



That's it! Just one requirement:



- **Docker Desktop** ([Download here](https://www.docker.com/products/docker-desktop))



> Docker handles everything: .NET SDK, SQL Server, dependencies, and configuration. No manual setup needed! 🎉



---



## 🚀 Getting Started (The "Magic" Way)



### 1️⃣ Clone the Repository

```bash

git clone https://github.com/ninjam5/Sha8alny.git

cd Sha8alny

```



### 2️⃣ Run with Docker Compose

```bash

docker-compose up --build

```



### 3️⃣ Wait for the Magic ✨

The application will:

- 🐳 Build the .NET 9 API container

- 🗄️ Spin up SQL Server 2022

- 🔄 **Automatically run database migrations**

- 🌱 **Seed demo data** (users, skills, universities, etc.)

- 🚀 Start the API server



### 4️⃣ Access the Application

Once you see `Now listening on: http://[::]:8080`, open:

- **Swagger UI**: [http://localhost:5000/swagger](http://localhost:5000/swagger)

- **API Base URL**: [http://localhost:5000](http://localhost:5000)



> **Note**: The first run may take 2-3 minutes as Docker downloads images and builds the project.



---



## 🔑 Default Demo Credentials



Use these pre-seeded accounts to explore the platform:



| Role      | Email                    | Password       | Description                          |

|-----------|--------------------------|----------------|--------------------------------------|

| 👨‍💼 **Admin**   | `admin@sha8alny.com`     | `Password123!` | Full system access and control       |

| 🏢 **Company** | `techcorp@test.com`      | `Password123!` | Post jobs and hire students          |

| 🎓 **Student** | `student@test.com`       | `Password123!` | Apply for jobs and complete projects |



> **Security Note**: Change these credentials before deploying to production!



---



## 📖 API Documentation



Comprehensive API documentation is available via **Swagger UI**:



🔗 **[http://localhost:5000/swagger](http://localhost:5000/swagger)**



### Available Endpoints:

- 🔐 **Authentication** - Login, Register, Refresh Tokens

- 👤 **User Management** - Profiles, Skills, Education

- 📋 **Projects** - CRUD operations, Search, Filter

- 📝 **Applications** - Apply, Accept, Reject, Track Progress

- 💬 **Chat** - Real-time messaging via SignalR

- 🔔 **Notifications** - Real-time updates

- 💳 **Payments** - Process payments and transactions

- ⭐ **Reviews** - Rate and review completed projects

- 👨‍💼 **Admin** - System management and analytics



> **Tip**: Use the "Authorize" button in Swagger UI to test authenticated endpoints.



---



## 📁 Project Structure



```

Sha8alny/

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

```



---



## 🛠️ Technology Stack



| Category            | Technology                          |

|---------------------|-------------------------------------|

| **Framework**       | ASP.NET Core 9 Web API              |

| **Language**        | C# 13                               |

| **Database**        | SQL Server 2022                     |

| **ORM**             | Entity Framework Core 9             |

| **Authentication**  | JWT (Access + Refresh Tokens)       |

| **Password Hashing**| BCrypt                              |

| **Real-time**       | SignalR                             |

| **Containerization**| Docker & Docker Compose             |

| **API Docs**        | Swagger/OpenAPI                     |

| **Mapping**         | AutoMapper                          |

| **Patterns**        | Repository, Unit of Work, DI        |



---



## 🔧 Advanced Configuration



### Environment Variables

You can customize the deployment by modifying `docker-compose.yml`:



```yaml

environment:

  - ASPNETCORE_ENVIRONMENT=Development

  - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=Sh8lnyDB;...

```



### Database Connection

The connection string is automatically configured in Docker Compose. For local development without Docker:



1. Update `appsettings.json`:

```json

"ConnectionStrings": {

  "DefaultConnection": "Server=localhost;Database=Sh8lnyDB;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"

}

```



2. Run migrations:

```bash

dotnet ef database update --project Infrastructure/Sh8lny.Persistence --startup-project Sh8lny.Web

```



---



## 🧪 Development



### Build the Solution

```bash

dotnet build

```



### Run Locally (without Docker)

```bash

cd Sh8lny.Web

dotnet run

```



### Create Migration

```bash

dotnet ef migrations add MigrationName --project Infrastructure/Sh8lny.Persistence --startup-project Sh8lny.Web

```



---



## 🤝 Contributing



Contributions are welcome! Please feel free to submit a Pull Request.



1. Fork the repository

2. Create your feature branch (`git checkout -b feature/AmazingFeature`)

3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)

4. Push to the branch (`git push origin feature/AmazingFeature`)

5. Open a Pull Request



---



## 📄 License



This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.



---



## 👥 Authors



- **Sha8alny Team** - *Initial work*



---



## 🙏 Acknowledgments



- Built with ❤️ using .NET 9

- Inspired by modern freelancing platforms

- Special thanks to the .NET community



---



## 📞 Support



For questions or support, please open an issue or contact the development team.



---



<div align="center">

  <p>Made with ❤️ in Egypt</p>

  <p>⭐ Star this repo if you find it helpful!</p>

</div>
