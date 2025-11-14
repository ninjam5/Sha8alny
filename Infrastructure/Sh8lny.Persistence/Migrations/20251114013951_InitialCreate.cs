using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Sh8lny.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    SkillID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SkillName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SkillCategory = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.SkillID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UserType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsEmailVerified = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    VerificationCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    VerificationCodeExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "ActivityLog",
                columns: table => new
                {
                    LogID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: true),
                    ActivityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RelatedEntityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RelatedEntityID = table.Column<int>(type: "int", nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityLog", x => x.LogID);
                    table.ForeignKey(
                        name: "FK_ActivityLog_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    CompanyID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CompanyLogo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ContactEmail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ContactPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Website = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Industry = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AverageRating = table.Column<decimal>(type: "decimal(3,2)", nullable: false, defaultValue: 0m),
                    TotalReviews = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.CompanyID);
                    table.ForeignKey(
                        name: "FK_Companies_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    NotificationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    NotificationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    RelatedProjectID = table.Column<int>(type: "int", nullable: true),
                    RelatedApplicationID = table.Column<int>(type: "int", nullable: true),
                    ActionURL = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.NotificationID);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Universities",
                columns: table => new
                {
                    UniversityID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: true),
                    UniversityName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UniversityLogo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ContactEmail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ContactPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Website = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UniversityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Universities", x => x.UniversityID);
                    table.ForeignKey(
                        name: "FK_Universities_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "UserSettings",
                columns: table => new
                {
                    SettingID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    EmailNotifications = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    PushNotifications = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    MessageNotifications = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ApplicationNotifications = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Language = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "English"),
                    Timezone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "UTC"),
                    ProfileVisibility = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Public"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.SettingID);
                    table.ForeignKey(
                        name: "FK_UserSettings_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    ProjectID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyID = table.Column<int>(type: "int", nullable: false),
                    ProjectName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ProjectCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ProjectType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deadline = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Duration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequiredSkills = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MinAcademicYear = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaxApplicants = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Draft"),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ViewCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ApplicationCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.ProjectID);
                    table.ForeignKey(
                        name: "FK_Projects_Companies_CompanyID",
                        column: x => x.CompanyID,
                        principalTable: "Companies",
                        principalColumn: "CompanyID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DashboardMetrics",
                columns: table => new
                {
                    MetricID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TotalStudents = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    TotalProjects = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CompletedProjects = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    AvailableOpportunities = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    NewApplicants = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ActivityIncreasePercent = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    MetricDate = table.Column<DateTime>(type: "date", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    CompanyID = table.Column<int>(type: "int", nullable: true),
                    UniversityID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashboardMetrics", x => x.MetricID);
                    table.ForeignKey(
                        name: "FK_DashboardMetrics_Companies_CompanyID",
                        column: x => x.CompanyID,
                        principalTable: "Companies",
                        principalColumn: "CompanyID");
                    table.ForeignKey(
                        name: "FK_DashboardMetrics_Universities_UniversityID",
                        column: x => x.UniversityID,
                        principalTable: "Universities",
                        principalColumn: "UniversityID");
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    DepartmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UniversityID = table.Column<int>(type: "int", nullable: false),
                    DepartmentName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.DepartmentID);
                    table.ForeignKey(
                        name: "FK_Departments_Universities_UniversityID",
                        column: x => x.UniversityID,
                        principalTable: "Universities",
                        principalColumn: "UniversityID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectGroups",
                columns: table => new
                {
                    GroupID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectID = table.Column<int>(type: "int", nullable: false),
                    GroupName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MaxMembers = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectGroups", x => x.GroupID);
                    table.ForeignKey(
                        name: "FK_ProjectGroups_Projects_ProjectID",
                        column: x => x.ProjectID,
                        principalTable: "Projects",
                        principalColumn: "ProjectID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectRequiredSkills",
                columns: table => new
                {
                    ProjectSkillID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectID = table.Column<int>(type: "int", nullable: false),
                    SkillID = table.Column<int>(type: "int", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectRequiredSkills", x => x.ProjectSkillID);
                    table.ForeignKey(
                        name: "FK_ProjectRequiredSkills_Projects_ProjectID",
                        column: x => x.ProjectID,
                        principalTable: "Projects",
                        principalColumn: "ProjectID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectRequiredSkills_Skills_SkillID",
                        column: x => x.SkillID,
                        principalTable: "Skills",
                        principalColumn: "SkillID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    StudentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ProfilePicture = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UniversityID = table.Column<int>(type: "int", nullable: true),
                    DepartmentID = table.Column<int>(type: "int", nullable: true),
                    AcademicYear = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    StudentIDNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, defaultValue: "Egypt"),
                    ProfileCompleteness = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Active"),
                    AverageRating = table.Column<decimal>(type: "decimal(3,2)", nullable: false, defaultValue: 0m),
                    TotalReviews = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.StudentID);
                    table.ForeignKey(
                        name: "FK_Students_Departments_DepartmentID",
                        column: x => x.DepartmentID,
                        principalTable: "Departments",
                        principalColumn: "DepartmentID");
                    table.ForeignKey(
                        name: "FK_Students_Universities_UniversityID",
                        column: x => x.UniversityID,
                        principalTable: "Universities",
                        principalColumn: "UniversityID");
                    table.ForeignKey(
                        name: "FK_Students_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Conversations",
                columns: table => new
                {
                    ConversationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConversationType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    GroupID = table.Column<int>(type: "int", nullable: true),
                    ConversationName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    LastMessageAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProjectID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversations", x => x.ConversationID);
                    table.ForeignKey(
                        name: "FK_Conversations_ProjectGroups_GroupID",
                        column: x => x.GroupID,
                        principalTable: "ProjectGroups",
                        principalColumn: "GroupID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Conversations_Projects_ProjectID",
                        column: x => x.ProjectID,
                        principalTable: "Projects",
                        principalColumn: "ProjectID");
                });

            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    ApplicationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectID = table.Column<int>(type: "int", nullable: false),
                    StudentID = table.Column<int>(type: "int", nullable: false),
                    CoverLetter = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Resume = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PortfolioURL = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ProposalDocument = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Submit"),
                    ReviewedBy = table.Column<int>(type: "int", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AppliedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.ApplicationID);
                    table.ForeignKey(
                        name: "FK_Applications_Projects_ProjectID",
                        column: x => x.ProjectID,
                        principalTable: "Projects",
                        principalColumn: "ProjectID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Applications_Students_StudentID",
                        column: x => x.StudentID,
                        principalTable: "Students",
                        principalColumn: "StudentID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Certificates",
                columns: table => new
                {
                    CertificateID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentID = table.Column<int>(type: "int", nullable: false),
                    ProjectID = table.Column<int>(type: "int", nullable: false),
                    CompanyID = table.Column<int>(type: "int", nullable: false),
                    CertificateNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CertificateTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CertificateURL = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IssuedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificates", x => x.CertificateID);
                    table.ForeignKey(
                        name: "FK_Certificates_Companies_CompanyID",
                        column: x => x.CompanyID,
                        principalTable: "Companies",
                        principalColumn: "CompanyID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Certificates_Projects_ProjectID",
                        column: x => x.ProjectID,
                        principalTable: "Projects",
                        principalColumn: "ProjectID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Certificates_Students_StudentID",
                        column: x => x.StudentID,
                        principalTable: "Students",
                        principalColumn: "StudentID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GroupMembers",
                columns: table => new
                {
                    GroupMemberID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupID = table.Column<int>(type: "int", nullable: false),
                    StudentID = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    JoinedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupMembers", x => x.GroupMemberID);
                    table.ForeignKey(
                        name: "FK_GroupMembers_ProjectGroups_GroupID",
                        column: x => x.GroupID,
                        principalTable: "ProjectGroups",
                        principalColumn: "GroupID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupMembers_Students_StudentID",
                        column: x => x.StudentID,
                        principalTable: "Students",
                        principalColumn: "StudentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    PaymentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectID = table.Column<int>(type: "int", nullable: false),
                    StudentID = table.Column<int>(type: "int", nullable: false),
                    CompanyID = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TransactionID = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PaymentGateway = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PaymentReference = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RefundedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.PaymentID);
                    table.ForeignKey(
                        name: "FK_Payments_Companies_CompanyID",
                        column: x => x.CompanyID,
                        principalTable: "Companies",
                        principalColumn: "CompanyID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Payments_Projects_ProjectID",
                        column: x => x.ProjectID,
                        principalTable: "Projects",
                        principalColumn: "ProjectID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payments_Students_StudentID",
                        column: x => x.StudentID,
                        principalTable: "Students",
                        principalColumn: "StudentID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudentSkills",
                columns: table => new
                {
                    StudentSkillID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentID = table.Column<int>(type: "int", nullable: false),
                    SkillID = table.Column<int>(type: "int", nullable: false),
                    ProficiencyLevel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentSkills", x => x.StudentSkillID);
                    table.ForeignKey(
                        name: "FK_StudentSkills_Skills_SkillID",
                        column: x => x.SkillID,
                        principalTable: "Skills",
                        principalColumn: "SkillID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentSkills_Students_StudentID",
                        column: x => x.StudentID,
                        principalTable: "Students",
                        principalColumn: "StudentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConversationParticipants",
                columns: table => new
                {
                    ParticipantID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConversationID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    LastReadAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversationParticipants", x => x.ParticipantID);
                    table.ForeignKey(
                        name: "FK_ConversationParticipants_Conversations_ConversationID",
                        column: x => x.ConversationID,
                        principalTable: "Conversations",
                        principalColumn: "ConversationID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConversationParticipants_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    MessageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConversationID = table.Column<int>(type: "int", nullable: false),
                    SenderID = table.Column<int>(type: "int", nullable: false),
                    MessageText = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    MessageType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Text"),
                    AttachmentURL = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AttachmentName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsEdited = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    EditedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.MessageID);
                    table.ForeignKey(
                        name: "FK_Messages_Conversations_ConversationID",
                        column: x => x.ConversationID,
                        principalTable: "Conversations",
                        principalColumn: "ConversationID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Messages_Users_SenderID",
                        column: x => x.SenderID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompletedOpportunities",
                columns: table => new
                {
                    CompletedOpportunityID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentID = table.Column<int>(type: "int", nullable: false),
                    ProjectID = table.Column<int>(type: "int", nullable: false),
                    ApplicationID = table.Column<int>(type: "int", nullable: true),
                    CertificateID = table.Column<int>(type: "int", nullable: true),
                    OpportunityTitle = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    OpportunityType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DurationInDays = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<decimal>(type: "decimal(3,2)", nullable: true),
                    StudentFeedback = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CompanyFeedback = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Achievements = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    SkillsGained = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    VerifiedBy = table.Column<int>(type: "int", nullable: true),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    TotalPayment = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsVisibleOnProfile = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompletedOpportunities", x => x.CompletedOpportunityID);
                    table.ForeignKey(
                        name: "FK_CompletedOpportunities_Applications_ApplicationID",
                        column: x => x.ApplicationID,
                        principalTable: "Applications",
                        principalColumn: "ApplicationID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CompletedOpportunities_Certificates_CertificateID",
                        column: x => x.CertificateID,
                        principalTable: "Certificates",
                        principalColumn: "CertificateID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CompletedOpportunities_Projects_ProjectID",
                        column: x => x.ProjectID,
                        principalTable: "Projects",
                        principalColumn: "ProjectID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompletedOpportunities_Students_StudentID",
                        column: x => x.StudentID,
                        principalTable: "Students",
                        principalColumn: "StudentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompletedOpportunities_Users_VerifiedBy",
                        column: x => x.VerifiedBy,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CompanyReviews",
                columns: table => new
                {
                    ReviewID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyID = table.Column<int>(type: "int", nullable: false),
                    StudentID = table.Column<int>(type: "int", nullable: false),
                    CompletedOpportunityID = table.Column<int>(type: "int", nullable: true),
                    Rating = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    ReviewTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewText = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    WorkEnvironmentRating = table.Column<decimal>(type: "decimal(3,2)", nullable: true),
                    LearningOpportunityRating = table.Column<decimal>(type: "decimal(3,2)", nullable: true),
                    MentorshipRating = table.Column<decimal>(type: "decimal(3,2)", nullable: true),
                    CompensationRating = table.Column<decimal>(type: "decimal(3,2)", nullable: true),
                    CommunicationRating = table.Column<decimal>(type: "decimal(3,2)", nullable: true),
                    WouldRecommend = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Pros = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Cons = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsAnonymous = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyResponse = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CompanyRespondedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyReviews", x => x.ReviewID);
                    table.ForeignKey(
                        name: "FK_CompanyReviews_Companies_CompanyID",
                        column: x => x.CompanyID,
                        principalTable: "Companies",
                        principalColumn: "CompanyID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyReviews_CompletedOpportunities_CompletedOpportunityID",
                        column: x => x.CompletedOpportunityID,
                        principalTable: "CompletedOpportunities",
                        principalColumn: "CompletedOpportunityID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CompanyReviews_Students_StudentID",
                        column: x => x.StudentID,
                        principalTable: "Students",
                        principalColumn: "StudentID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudentReviews",
                columns: table => new
                {
                    ReviewID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentID = table.Column<int>(type: "int", nullable: false),
                    CompanyID = table.Column<int>(type: "int", nullable: false),
                    CompletedOpportunityID = table.Column<int>(type: "int", nullable: true),
                    Rating = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    ReviewTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewText = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    TechnicalSkillsRating = table.Column<decimal>(type: "decimal(3,2)", nullable: true),
                    CommunicationRating = table.Column<decimal>(type: "decimal(3,2)", nullable: true),
                    ProfessionalismRating = table.Column<decimal>(type: "decimal(3,2)", nullable: true),
                    TimeManagementRating = table.Column<decimal>(type: "decimal(3,2)", nullable: true),
                    TeamworkRating = table.Column<decimal>(type: "decimal(3,2)", nullable: true),
                    ProblemSolvingRating = table.Column<decimal>(type: "decimal(3,2)", nullable: true),
                    WouldHireAgain = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Strengths = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AreasForImprovement = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StudentResponse = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    StudentRespondedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentReviews", x => x.ReviewID);
                    table.ForeignKey(
                        name: "FK_StudentReviews_Companies_CompanyID",
                        column: x => x.CompanyID,
                        principalTable: "Companies",
                        principalColumn: "CompanyID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentReviews_CompletedOpportunities_CompletedOpportunityID",
                        column: x => x.CompletedOpportunityID,
                        principalTable: "CompletedOpportunities",
                        principalColumn: "CompletedOpportunityID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_StudentReviews_Students_StudentID",
                        column: x => x.StudentID,
                        principalTable: "Students",
                        principalColumn: "StudentID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Skills",
                columns: new[] { "SkillID", "Description", "SkillCategory", "SkillName" },
                values: new object[,]
                {
                    { 1, null, "Backend", "Backend" },
                    { 2, null, "Frontend", "Frontend" },
                    { 3, null, "UIUX", "UI&UX" },
                    { 4, null, "Mobile", "Mobile App" },
                    { 5, null, "Frontend", "Web Development" },
                    { 6, null, "AIML", "AI" },
                    { 7, null, "Data", "Big Data" },
                    { 8, null, "Testing", "Testing" },
                    { 9, null, "Marketing", "Marketing" },
                    { 10, null, "Marketing", "Social Media" },
                    { 11, null, "Design", "Photoshop" },
                    { 12, null, "Security", "Cybersecurity" },
                    { 13, null, "Mobile", "Flutter" }
                });

            migrationBuilder.CreateIndex(
                name: "IDX_ActivityLog_ActivityType",
                table: "ActivityLog",
                column: "ActivityType");

            migrationBuilder.CreateIndex(
                name: "IDX_ActivityLog_CreatedAt",
                table: "ActivityLog",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IDX_ActivityLog_RelatedEntity",
                table: "ActivityLog",
                columns: new[] { "RelatedEntityType", "RelatedEntityID" });

            migrationBuilder.CreateIndex(
                name: "IDX_ActivityLog_UserID",
                table: "ActivityLog",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IDX_Applications_AppliedAt",
                table: "Applications",
                column: "AppliedAt");

            migrationBuilder.CreateIndex(
                name: "IDX_Applications_ProjectID",
                table: "Applications",
                column: "ProjectID");

            migrationBuilder.CreateIndex(
                name: "IDX_Applications_Status",
                table: "Applications",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IDX_Applications_StudentID",
                table: "Applications",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "UQ_Application_ProjectID_StudentID",
                table: "Applications",
                columns: new[] { "ProjectID", "StudentID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IDX_Certificates_CertificateNumber",
                table: "Certificates",
                column: "CertificateNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IDX_Certificates_CompanyID",
                table: "Certificates",
                column: "CompanyID");

            migrationBuilder.CreateIndex(
                name: "IDX_Certificates_ProjectID",
                table: "Certificates",
                column: "ProjectID");

            migrationBuilder.CreateIndex(
                name: "IDX_Certificates_StudentID",
                table: "Certificates",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "IDX_Companies_AverageRating",
                table: "Companies",
                column: "AverageRating");

            migrationBuilder.CreateIndex(
                name: "IDX_Companies_UserID",
                table: "Companies",
                column: "UserID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IDX_CompanyReviews_CompanyID",
                table: "CompanyReviews",
                column: "CompanyID");

            migrationBuilder.CreateIndex(
                name: "IDX_CompanyReviews_CompletedOpportunityID",
                table: "CompanyReviews",
                column: "CompletedOpportunityID",
                unique: true,
                filter: "[CompletedOpportunityID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IDX_CompanyReviews_CreatedAt",
                table: "CompanyReviews",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IDX_CompanyReviews_Rating",
                table: "CompanyReviews",
                column: "Rating");

            migrationBuilder.CreateIndex(
                name: "IDX_CompanyReviews_Status",
                table: "CompanyReviews",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IDX_CompanyReviews_StudentID",
                table: "CompanyReviews",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "UQ_CompanyReviews_StudentID_CompletedOpportunityID",
                table: "CompanyReviews",
                columns: new[] { "StudentID", "CompletedOpportunityID" },
                unique: true,
                filter: "[CompletedOpportunityID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IDX_CompletedOpportunities_ApplicationID",
                table: "CompletedOpportunities",
                column: "ApplicationID",
                unique: true,
                filter: "[ApplicationID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IDX_CompletedOpportunities_CertificateID",
                table: "CompletedOpportunities",
                column: "CertificateID",
                unique: true,
                filter: "[CertificateID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IDX_CompletedOpportunities_CompletedAt",
                table: "CompletedOpportunities",
                column: "CompletedAt");

            migrationBuilder.CreateIndex(
                name: "IDX_CompletedOpportunities_IsVerified",
                table: "CompletedOpportunities",
                column: "IsVerified");

            migrationBuilder.CreateIndex(
                name: "IDX_CompletedOpportunities_OpportunityType",
                table: "CompletedOpportunities",
                column: "OpportunityType");

            migrationBuilder.CreateIndex(
                name: "IDX_CompletedOpportunities_ProjectID",
                table: "CompletedOpportunities",
                column: "ProjectID");

            migrationBuilder.CreateIndex(
                name: "IDX_CompletedOpportunities_Status",
                table: "CompletedOpportunities",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IDX_CompletedOpportunities_StudentID",
                table: "CompletedOpportunities",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "IX_CompletedOpportunities_VerifiedBy",
                table: "CompletedOpportunities",
                column: "VerifiedBy");

            migrationBuilder.CreateIndex(
                name: "IDX_ConversationParticipants_ConversationID",
                table: "ConversationParticipants",
                column: "ConversationID");

            migrationBuilder.CreateIndex(
                name: "IDX_ConversationParticipants_ConversationID_UserID",
                table: "ConversationParticipants",
                columns: new[] { "ConversationID", "UserID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IDX_ConversationParticipants_UserID",
                table: "ConversationParticipants",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IDX_Conversations_ConversationType",
                table: "Conversations",
                column: "ConversationType");

            migrationBuilder.CreateIndex(
                name: "IDX_Conversations_GroupID",
                table: "Conversations",
                column: "GroupID");

            migrationBuilder.CreateIndex(
                name: "IDX_Conversations_LastMessageAt",
                table: "Conversations",
                column: "LastMessageAt");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_ProjectID",
                table: "Conversations",
                column: "ProjectID");

            migrationBuilder.CreateIndex(
                name: "IDX_DashboardMetrics_MetricDate",
                table: "DashboardMetrics",
                column: "MetricDate");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardMetrics_CompanyID",
                table: "DashboardMetrics",
                column: "CompanyID");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardMetrics_UniversityID",
                table: "DashboardMetrics",
                column: "UniversityID");

            migrationBuilder.CreateIndex(
                name: "IDX_Departments_IsActive",
                table: "Departments",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IDX_Departments_UniversityID",
                table: "Departments",
                column: "UniversityID");

            migrationBuilder.CreateIndex(
                name: "IDX_GroupMembers_GroupID",
                table: "GroupMembers",
                column: "GroupID");

            migrationBuilder.CreateIndex(
                name: "IDX_GroupMembers_GroupID_StudentID",
                table: "GroupMembers",
                columns: new[] { "GroupID", "StudentID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IDX_GroupMembers_StudentID",
                table: "GroupMembers",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "IDX_Messages_ConversationID",
                table: "Messages",
                column: "ConversationID");

            migrationBuilder.CreateIndex(
                name: "IDX_Messages_IsRead",
                table: "Messages",
                column: "IsRead");

            migrationBuilder.CreateIndex(
                name: "IDX_Messages_SenderID",
                table: "Messages",
                column: "SenderID");

            migrationBuilder.CreateIndex(
                name: "IDX_Messages_SentAt",
                table: "Messages",
                column: "SentAt");

            migrationBuilder.CreateIndex(
                name: "IDX_Notifications_CreatedAt",
                table: "Notifications",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IDX_Notifications_IsRead",
                table: "Notifications",
                column: "IsRead");

            migrationBuilder.CreateIndex(
                name: "IDX_Notifications_NotificationType",
                table: "Notifications",
                column: "NotificationType");

            migrationBuilder.CreateIndex(
                name: "IDX_Notifications_UserID",
                table: "Notifications",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IDX_Payments_CompanyID",
                table: "Payments",
                column: "CompanyID");

            migrationBuilder.CreateIndex(
                name: "IDX_Payments_CreatedAt",
                table: "Payments",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IDX_Payments_ProjectID",
                table: "Payments",
                column: "ProjectID");

            migrationBuilder.CreateIndex(
                name: "IDX_Payments_Status",
                table: "Payments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IDX_Payments_StudentID",
                table: "Payments",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "IDX_Payments_TransactionID",
                table: "Payments",
                column: "TransactionID");

            migrationBuilder.CreateIndex(
                name: "IDX_ProjectGroups_ProjectID",
                table: "ProjectGroups",
                column: "ProjectID");

            migrationBuilder.CreateIndex(
                name: "IDX_ProjectSkills_ProjectID",
                table: "ProjectRequiredSkills",
                column: "ProjectID");

            migrationBuilder.CreateIndex(
                name: "IDX_ProjectSkills_SkillID",
                table: "ProjectRequiredSkills",
                column: "SkillID");

            migrationBuilder.CreateIndex(
                name: "UQ_ProjectSkills_ProjectID_SkillID",
                table: "ProjectRequiredSkills",
                columns: new[] { "ProjectID", "SkillID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IDX_Projects_CompanyID",
                table: "Projects",
                column: "CompanyID");

            migrationBuilder.CreateIndex(
                name: "IDX_Projects_Deadline",
                table: "Projects",
                column: "Deadline");

            migrationBuilder.CreateIndex(
                name: "IDX_Projects_IsVisible",
                table: "Projects",
                column: "IsVisible");

            migrationBuilder.CreateIndex(
                name: "IDX_Projects_ProjectType",
                table: "Projects",
                column: "ProjectType");

            migrationBuilder.CreateIndex(
                name: "IDX_Projects_Status",
                table: "Projects",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "UQ_Projects_ProjectCode",
                table: "Projects",
                column: "ProjectCode",
                unique: true,
                filter: "[ProjectCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IDX_Skills_IsActive",
                table: "Skills",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IDX_Skills_SkillCategory",
                table: "Skills",
                column: "SkillCategory");

            migrationBuilder.CreateIndex(
                name: "UQ_Skills_SkillName",
                table: "Skills",
                column: "SkillName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IDX_StudentReviews_CompanyID",
                table: "StudentReviews",
                column: "CompanyID");

            migrationBuilder.CreateIndex(
                name: "IDX_StudentReviews_CompletedOpportunityID",
                table: "StudentReviews",
                column: "CompletedOpportunityID",
                unique: true,
                filter: "[CompletedOpportunityID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IDX_StudentReviews_CreatedAt",
                table: "StudentReviews",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IDX_StudentReviews_Rating",
                table: "StudentReviews",
                column: "Rating");

            migrationBuilder.CreateIndex(
                name: "IDX_StudentReviews_Status",
                table: "StudentReviews",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IDX_StudentReviews_StudentID",
                table: "StudentReviews",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "UQ_StudentReviews_CompanyID_CompletedOpportunityID",
                table: "StudentReviews",
                columns: new[] { "CompanyID", "CompletedOpportunityID" },
                unique: true,
                filter: "[CompletedOpportunityID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IDX_Students_AverageRating",
                table: "Students",
                column: "AverageRating");

            migrationBuilder.CreateIndex(
                name: "IDX_Students_DepartmentID",
                table: "Students",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IDX_Students_Status",
                table: "Students",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IDX_Students_UniversityID",
                table: "Students",
                column: "UniversityID");

            migrationBuilder.CreateIndex(
                name: "IDX_Students_UserID",
                table: "Students",
                column: "UserID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IDX_StudentSkills_SkillID",
                table: "StudentSkills",
                column: "SkillID");

            migrationBuilder.CreateIndex(
                name: "IDX_StudentSkills_StudentID",
                table: "StudentSkills",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "UQ_StudentSkills_StudentID_SkillID",
                table: "StudentSkills",
                columns: new[] { "StudentID", "SkillID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IDX_Universities_IsActive",
                table: "Universities",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IDX_Universities_UserID",
                table: "Universities",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IDX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IDX_Users_UserType",
                table: "Users",
                column: "UserType");

            migrationBuilder.CreateIndex(
                name: "IDX_UserSettings_UserID",
                table: "UserSettings",
                column: "UserID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityLog");

            migrationBuilder.DropTable(
                name: "CompanyReviews");

            migrationBuilder.DropTable(
                name: "ConversationParticipants");

            migrationBuilder.DropTable(
                name: "DashboardMetrics");

            migrationBuilder.DropTable(
                name: "GroupMembers");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "ProjectRequiredSkills");

            migrationBuilder.DropTable(
                name: "StudentReviews");

            migrationBuilder.DropTable(
                name: "StudentSkills");

            migrationBuilder.DropTable(
                name: "UserSettings");

            migrationBuilder.DropTable(
                name: "Conversations");

            migrationBuilder.DropTable(
                name: "CompletedOpportunities");

            migrationBuilder.DropTable(
                name: "Skills");

            migrationBuilder.DropTable(
                name: "ProjectGroups");

            migrationBuilder.DropTable(
                name: "Applications");

            migrationBuilder.DropTable(
                name: "Certificates");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Universities");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
