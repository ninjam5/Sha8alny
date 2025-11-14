# What The Fuck - Sha8lny Platform Models & Configurations Documentation

This document explains what each model does and how its Fluent API configuration works in the Sha8lny student training and internship management platform.

---

## 1. User Model

**Purpose**: Central authentication and account management entity for all platform users.

**What it does**:
- Stores core authentication data (email, password hash, verification codes)
- Supports multiple user types: Student, Company, University, Admin
- Manages email verification and account status
- Tracks user activity (creation, last login, updates)

**Configuration Highlights**:
- Table: `Users`
- Unique email index for fast lookups and preventing duplicates
- Indexes on `UserType` and `IsActive` for filtering queries
- One-to-one relationships with Student, Company, University, and UserSettings
- One-to-many relationships with ConversationParticipants, Messages, Notifications, ActivityLogs
- Default values: `IsEmailVerified = false`, `IsActive = true`, `CreatedAt/UpdatedAt = GETDATE()`

---

## 2. Student Model

**Purpose**: Stores detailed profile information for student users.

**What it does**:
- Manages student personal information (name, phone, profile picture)
- Links students to universities and departments
- Tracks academic year and student status (Active, Inactive, Suspended, Graduated)
- Stores location data with default country as "Egypt"
- Calculates profile completeness percentage
- Provides computed `FullName` property (not stored in DB)
- **Maintains average rating score based on company reviews**
- **Tracks total number of reviews received from companies**

**Configuration Highlights**:
- Table: `Students`
- `FullName` property is ignored (computed property, not mapped to DB)
- Indexes on `UserID`, `UniversityID`, `DepartmentID`, `Status`, `AverageRating` for efficient queries
- Relationships: University, Department (SetNull on delete), StudentSkills, Applications, GroupMemberships, Certificates, ReceivedReviews collection
- Default values: `Country = "Egypt"`, `ProfileCompleteness = 0`, `Status = Active`, `AverageRating = 0`, `TotalReviews = 0`
- `AverageRating` uses decimal(3,2) for precision (e.g., 4.75 out of 5.00)

---

## 3. Company Model

**Purpose**: Manages company profiles that post projects and internship opportunities.

**What it does**:
- Stores company information (name, logo, contact details, website)
- Manages company verification status
- Tracks partnership tiers (Basic, Premium, Enterprise)
- Stores company size categories (Startup to Corporation)
- Maintains company status (Active, Inactive, PendingApproval, Suspended)
- **Maintains average rating score based on student reviews**
- **Tracks total number of reviews received**

**Configuration Highlights**:
- Table: `Companies`
- Indexes on `UserID`, `AverageRating` for filtering and searching
- Relationships: Projects, IssuedCertificates (Restrict on delete), DashboardMetrics (Cascade), Reviews collection
- Enums converted to strings: `CompanySize`, `PartnershipTier`, `CompanyStatus`
- Default values: `IsVerified = false`, `Status = Active`, `AverageRating = 0`, `TotalReviews = 0`
- `AverageRating` uses decimal(3,2) for precision (e.g., 4.75 out of 5.00)

---

## 4. University Model

**Purpose**: Manages university profiles and their relationship with students.

**What it does**:
- Stores university information (name, logo, contact details, location)
- Supports different university types (Public, Private, International)
- Optional link to User account (some universities may not have login accounts)
- Manages active/inactive status

**Configuration Highlights**:
- Table: `Universities`
- Optional `UserID` foreign key (nullable)
- Indexes on `UserID` and `IsActive`
- Relationships: Departments (Cascade), Students, DashboardMetrics
- `UniversityType` enum stored as string
- Default value: `IsActive = true`

---

## 5. Department Model

**Purpose**: Organizes students within universities by academic departments.

**What it does**:
- Stores department details (name, code, description)
- Links departments to their parent university
- Manages department status (active/inactive)

**Configuration Highlights**:
- Table: `Departments`
- Indexes on `UniversityID` and `IsActive`
- Relationship: University (Cascade delete - if university deleted, departments are too)
- Relationship: Students collection
- Default value: `IsActive = true`

---

## 6. Skill Model

**Purpose**: Master catalog of skills that students can have and projects can require.

**What it does**:
- Maintains centralized list of available skills
- Categorizes skills (Backend, Frontend, UI/UX, Mobile, AI/ML, Data, Testing, Marketing, Design, Security, Other)
- Manages skill descriptions and active status
- Prevents duplicate skills via unique skill name

**Configuration Highlights**:
- Table: `Skills`
- Unique index on `SkillName` prevents duplicates
- Indexes on `SkillCategory` and `IsActive` for filtering
- `SkillCategory` enum stored as string
- Relationships: StudentSkills, ProjectRequiredSkills (many-to-many junction tables)
- Default value: `IsActive = true`

---

## 7. StudentSkill Model (Junction Table)

**Purpose**: Links students to their skills with proficiency levels.

**What it does**:
- Creates many-to-many relationship between Students and Skills
- Stores proficiency level (Beginner, Intermediate, Advanced, Expert)
- Prevents duplicate skill assignments to the same student
- Tracks when skill was added

**Configuration Highlights**:
- Table: `StudentSkills`
- Primary key: `StudentSkillID`
- Unique composite index on `(StudentID, SkillID)` prevents duplicates
- Indexes on both foreign keys for query performance
- `ProficiencyLevel` enum stored as string
- Delete behavior: Cascade on Student, Restrict on Skill (prevent accidental skill deletion)

---

## 8. Project Model

**Purpose**: Represents job postings, internships, training programs, and graduation projects.

**What it does**:
- Stores project details (name, code, description, dates, deadlines)
- Categorizes projects by type (Internship, GraduationProject, Training, PartTime, FullTime)
- Manages project status (Draft, Active, Pending, Complete, Cancelled, Closed)
- Tracks required skills, academic year requirements, max applicants
- Stores creator information and tracks view/application counts
- Controls project visibility

**Configuration Highlights**:
- Table: `Projects`
- Unique index on `ProjectCode`
- Indexes on `CompanyID`, `Status`, `Deadline`, `ProjectType`, `IsVisible`
- `ProjectType` and `Status` enums stored as strings
- Relationships: Company (Restrict), ProjectRequiredSkills, Applications, ProjectGroups, Conversations, Certificates
- Default values: `Status = Draft`, `IsVisible = true`, `ViewCount = 0`, `ApplicationCount = 0`

---

## 9. ProjectRequiredSkill Model (Junction Table)

**Purpose**: Links projects to their required skills.

**What it does**:
- Creates many-to-many relationship between Projects and Skills
- Marks skills as required or optional for the project
- Prevents duplicate skill requirements for the same project

**Configuration Highlights**:
- Table: `ProjectRequiredSkills`
- Primary key: `ProjectSkillID`
- Unique composite index on `(ProjectID, SkillID)` prevents duplicates
- Delete behavior: Cascade on Project, Restrict on Skill
- Default value: `IsRequired = true`

---

## 10. Application Model

**Purpose**: Manages student applications to projects.

**What it does**:
- Stores application details (cover letter, resume, portfolio URL)
- Tracks application status (Submit, Pending, UnderReview, Accepted, Rejected, Withdrawn)
- Records student's time preference (AM, PM, Flexible)
- Stores review information (reviewer, notes, review date)
- Prevents duplicate applications (one student per project)

**Configuration Highlights**:
- Table: `Applications`
- Unique composite index on `(ProjectID, StudentID)` ensures one application per student per project
- Indexes on `ProjectID`, `StudentID`, `Status`, `AppliedAt`
- `Status` and `TimePreference` enums stored as strings
- Relationships: Project (Restrict), Student (Restrict), Reviewer/User (SetNull)
- Default value: `Status = Submit`

---

## 11. Certificate Model

**Purpose**: Manages achievement certificates issued to students.

**What it does**:
- Stores certificate details (number, title, description, URL)
- Links certificates to students, projects, and issuing companies
- Manages verification status
- Tracks certificate issuance and expiration dates
- Enforces unique certificate numbers

**Configuration Highlights**:
- Table: `Certificates`
- Unique index on `CertificateNumber`
- Indexes on `StudentID`, `ProjectID`, `CompanyID`, `IsVerified`
- Relationships: Student (Restrict), Project (Restrict), Company (Restrict)
- Default values: `IsVerified = false`, `IssuedAt = GETDATE()`

---

## 12. Notification Model

**Purpose**: Manages in-app notifications for users.

**What it does**:
- Sends notifications about applications, messages, projects, deadlines, etc.
- Categorizes notifications by type (Application, Message, Project, Deadline, Acceptance, Rejection, System)
- Stores notification content (title, message, action URL)
- Links to related entities (projects, applications)
- Tracks read status

**Configuration Highlights**:
- Table: `Notifications`
- Indexes on `UserID`, `IsRead`, `NotificationType`, `CreatedAt`
- `NotificationType` enum stored as string
- Relationship: User (Cascade delete)
- Default values: `IsRead = false`, `CreatedAt = GETDATE()`

---

## 13. Conversation Model

**Purpose**: Manages conversation threads for messaging.

**What it does**:
- Creates conversation containers for messages
- Supports different types (Direct, Group, Project)
- Links conversations to projects or groups
- Tracks conversation creation and last message time

**Configuration Highlights**:
- Table: `Conversations`
- Indexes on `ConversationType`, `ProjectID`, `GroupID`, `LastMessageAt`
- `ConversationType` enum stored as string
- Relationships: Project (SetNull), Group (Cascade), Participants, Messages
- Default value: `CreatedAt = GETDATE()`

---

## 14. ConversationParticipant Model (Junction Table)

**Purpose**: Manages user membership in conversations.

**What it does**:
- Creates many-to-many relationship between Conversations and Users
- Prevents duplicate participation (one user per conversation once)
- Tracks when user joined and last read time

**Configuration Highlights**:
- Table: `ConversationParticipants`
- Primary key: `ParticipantID`
- Unique composite index on `(ConversationID, UserID)` prevents duplicate participants
- Indexes on both foreign keys
- Delete behavior: Cascade on both Conversation and User
- Default value: `JoinedAt = GETDATE()`

---

## 15. Message Model

**Purpose**: Stores individual messages within conversations.

**What it does**:
- Stores message content and sender information
- Supports different message types (Text, File, Image, Link)
- Handles file attachments (URL and name)
- Tracks read and edited status
- Records message timestamps

**Configuration Highlights**:
- Table: `Messages`
- Indexes on `ConversationID`, `SenderID`, `SentAt`, `IsRead`
- `MessageType` enum stored as string
- Relationship: Conversation (Cascade), Sender/User (no action to prevent message loss if user deleted)
- Default values: `MessageType = Text`, `IsRead = false`, `IsEdited = false`, `SentAt = GETDATE()`

---

## 16. ProjectGroup Model

**Purpose**: Manages team groups for projects.

**What it does**:
- Creates teams/groups within projects
- Stores group information (name, description, max members)
- Links groups to their parent project

**Configuration Highlights**:
- Table: `ProjectGroups`
- Index on `ProjectID`
- Relationships: Project (Cascade), GroupMembers collection, Conversations
- Default value: `CreatedAt = GETDATE()`

---

## 17. GroupMember Model (Junction Table)

**Purpose**: Manages student membership in project groups.

**What it does**:
- Creates many-to-many relationship between ProjectGroups and Students
- Prevents duplicate memberships (one student per group once)
- Stores member role
- Tracks join date

**Configuration Highlights**:
- Table: `GroupMembers`
- Primary key: `GroupMemberID`
- Unique composite index on `(GroupID, StudentID)` prevents duplicate memberships
- Indexes on both foreign keys
- Delete behavior: Cascade on both ProjectGroup and Student
- Default value: `JoinedAt = GETDATE()`

---

## 18. ActivityLog Model

**Purpose**: Audit trail for tracking user activities.

**What it does**:
- Logs user actions across the platform
- Stores activity type, description, related entity information
- Captures IP address and user agent for security
- Optional link to user (supports anonymous activities)

**Configuration Highlights**:
- Table: `ActivityLog`
- Optional `UserID` (nullable) allows logging non-authenticated activities
- Indexes on `UserID`, `ActivityType`, `CreatedAt`, and composite index on `(RelatedEntityType, RelatedEntityID)`
- Relationship: User (SetNull on delete to preserve audit history)
- Default value: `CreatedAt = GETDATE()`

---

## 19. DashboardMetric Model

**Purpose**: Caches analytics and metrics for dashboard displays.

**What it does**:
- Stores aggregated statistics for companies and universities
- Tracks student counts, project counts, completion rates
- Records available opportunities and new applicants
- Calculates activity increase percentages
- Links metrics to specific dates for historical tracking

**Configuration Highlights**:
- Table: `DashboardMetrics`
- Optional `CompanyID` and `UniversityID` (metrics can be for either)
- Indexes on `CompanyID`, `UniversityID`, `MetricDate`
- `ActivityIncreasePercent` uses decimal(5,2) for precision
- `MetricDate` stored as date type
- Relationships: Company (Cascade), University (Cascade)
- Default values: All counts default to 0, `CreatedAt = GETDATE()`

---

## 20. UserSettings Model

**Purpose**: Stores user preferences and settings.

**What it does**:
- Manages notification preferences (email, push, messages, applications)
- Stores language and timezone settings
- Controls profile visibility (Public, Private, UniversityOnly)
- One-to-one relationship with User (each user has one settings record)

**Configuration Highlights**:
- Table: `UserSettings`
- Unique index on `UserID` enforces one-to-one relationship
- `ProfileVisibility` enum stored as string
- Relationship: User (one-to-one)
- Default values: All notification flags = true, `Language = "English"`, `Timezone = "UTC"`, `ProfileVisibility = Public`, `UpdatedAt = GETDATE()`

---

## Database Design Patterns Used

### Delete Behaviors
- **Cascade**: Used for owned/dependent entities (if parent deleted, children should be too)
  - Example: User → Notifications, Company → DashboardMetrics
- **Restrict**: Used for independent entities (prevent accidental deletion)
  - Example: Project → Applications, Student → Applications
- **SetNull**: Used for optional relationships (preserve records but remove link)
  - Example: Application → Reviewer, ActivityLog → User

### Index Strategy
- **Unique indexes**: Prevent duplicate data (emails, certificate numbers, skill names)
- **Foreign key indexes**: Optimize join performance
- **Composite indexes**: Enforce business rules (one application per student per project)
- **Filter indexes**: Speed up common queries (Status, IsActive, CreatedAt)

### Default Values
- **Timestamps**: `GETDATE()` for SQL Server automatic timestamps
- **Booleans**: `false` for flags like `IsVerified`, `IsRead`, `IsActive = true`
- **Enums**: Default to most common value (e.g., `Status = Draft`, `Country = "Egypt"`)
- **Counters**: `0` for counts (ViewCount, ApplicationCount, all metrics)

### Enum Storage
All enums are converted to strings using `HasConversion<string>()` for:
- Better readability in database
- Easier querying and debugging
- Protection against enum value reordering

---

## 21. Payment Model

**Purpose**: Manages payment transactions for freelance projects and paid opportunities.

**What it does**:
- Records payment transactions between students, companies, and projects
- Tracks payment amounts in different currencies
- Manages payment status (Pending, Processing, Completed, Failed, Refunded, Cancelled)
- Supports multiple payment methods (CreditCard, DebitCard, BankTransfer, PayPal, Stripe, Cash, Wallet)
- Stores transaction IDs, payment gateway details, and references
- Records payment timestamps (created, paid, refunded)
- Includes notes and descriptions for transaction details

**Configuration Highlights**:
- Table: `Payments`
- `Amount` uses decimal(18,2) for currency precision
- Indexes on `ProjectID`, `StudentID`, `CompanyID`, `Status`, `TransactionID`, `CreatedAt`
- `Status` and `PaymentMethod` enums stored as strings
- Relationships: Project (Restrict), Student (Restrict), Company (SetNull)
- Default values: `CreatedAt/UpdatedAt = GETDATE()`

---

## 22. CompletedOpportunity Model

**Purpose**: Tracks finished internships, freelance jobs, training programs, and graduation projects.

**What it does**:
- Records completed opportunities for each student with full details
- Links to the original project, application, and certificate (if issued)
- Stores timeline information (start date, end date, duration in days)
- Captures performance ratings and feedback from both student and company
- Documents achievements and skills gained during the opportunity
- Tracks completion status (Completed, PartiallyCompleted, Terminated, OnHold, UnderReview)
- Manages verification by authorized users (admin, company, university)
- Records payment information (if opportunity was paid)
- Controls visibility on student's public profile
- Supports multiple opportunity types (Internship, FreelanceJob, Training, GraduationProject, PartTimeJob, FullTimeJob, Volunteering, Workshop)

**Configuration Highlights**:
- Table: `CompletedOpportunities`
- `Rating` uses decimal(3,2) for precision (e.g., 4.75 out of 5)
- `TotalPayment` uses decimal(18,2) for currency precision
- Indexes on `StudentID`, `ProjectID`, `ApplicationID`, `CertificateID`, `OpportunityType`, `Status`, `IsVerified`, `CompletedAt`
- `OpportunityType` and `CompletionStatus` enums stored as strings
- Relationships:
  - Student (Restrict) - preserve completed opportunities even if student deleted
  - Project (Restrict) - preserve record even if project deleted
  - Application (SetNull, one-to-one) - optional link to original application
  - Certificate (SetNull, one-to-one) - optional link to issued certificate
  - Verifier/User (SetNull) - optional link to user who verified the completion
  - CompanyReview (SetNull, one-to-one) - optional link to student's review of the company
- Default values: `IsVerified = false`, `IsPaid = false`, `IsVisibleOnProfile = true`, `CreatedAt/UpdatedAt = GETDATE()`

---

## 23. CompanyReview Model

**Purpose**: Manages student reviews and ratings of companies after completing opportunities.

**What it does**:
- Allows students to rate and review companies after finishing internships/projects
- Stores overall rating (1-5 scale with 2 decimal precision)
- Captures detailed breakdown ratings:
  - Work Environment Rating
  - Learning Opportunity Rating
  - Mentorship Rating
  - Compensation Rating
  - Communication Rating
- Records review title and detailed text feedback
- Documents pros and cons of working with the company
- Tracks whether student would recommend the company
- Supports anonymous reviews (student identity hidden)
- Manages review status (Pending, Approved, Rejected, Flagged)
- Allows companies to respond to reviews
- Links to the completed opportunity that triggered the review
- Enforces one review per student per completed opportunity

**Configuration Highlights**:
- Table: `CompanyReviews`
- All rating fields use decimal(3,2) for precision (e.g., 4.75 out of 5.00)
- Unique composite index on `(StudentID, CompletedOpportunityID)` with filter for non-null opportunities - ensures one review per completed opportunity
- Indexes on `CompanyID`, `StudentID`, `CompletedOpportunityID`, `Status`, `Rating`, `CreatedAt`
- `Status` enum stored as string
- Relationships:
  - Company (Restrict) - preserve reviews even if company deleted for historical record
  - Student (Restrict) - preserve reviews even if student deleted
  - CompletedOpportunity (SetNull, one-to-one) - optional link to the opportunity being reviewed
- Default values: `WouldRecommend = true`, `IsVerified = false`, `IsAnonymous = false`, `CreatedAt = GETDATE()`

**Business Logic**:
- After a student completes an opportunity, they can submit a review
- Company's `AverageRating` is calculated from all approved reviews
- Company's `TotalReviews` counter is updated when reviews are approved/rejected
- Reviews can be moderated before appearing on company profile
- Companies can respond to reviews to address feedback
- Anonymous reviews hide student identity but still count toward ratings

---

## 24. StudentReview Model

**Purpose**: Manages company reviews and ratings of students after completing opportunities.

**What it does**:
- Allows companies to rate and review students after finishing internships/projects
- Stores overall rating (1-5 scale with 2 decimal precision)
- Captures detailed breakdown ratings:
  - Technical Skills Rating
  - Communication Rating
  - Professionalism Rating
  - Time Management Rating
  - Teamwork Rating
  - Problem Solving Rating
- Records review title and detailed text feedback
- Documents student strengths and areas for improvement
- Tracks whether company would hire the student again
- Manages review status (Pending, Approved, Rejected, Flagged)
- Allows students to respond to reviews
- Controls visibility (IsPublic flag for profile display)
- Links to the completed opportunity that triggered the review
- Enforces one review per company per completed opportunity

**Configuration Highlights**:
- Table: `StudentReviews`
- All rating fields use decimal(3,2) for precision (e.g., 4.75 out of 5.00)
- Unique composite index on `(CompanyID, CompletedOpportunityID)` with filter for non-null opportunities - ensures one review per completed opportunity
- Indexes on `StudentID`, `CompanyID`, `CompletedOpportunityID`, `Status`, `Rating`, `CreatedAt`
- `Status` enum stored as string
- Relationships:
  - Student (Restrict) - preserve reviews even if student deleted for historical record
  - Company (Restrict) - preserve reviews even if company deleted
  - CompletedOpportunity (SetNull, one-to-one) - optional link to the opportunity being reviewed
- Default values: `WouldHireAgain = true`, `IsVerified = false`, `IsPublic = true`, `CreatedAt = GETDATE()`

**Business Logic**:
- After a student completes an opportunity, the company can submit a review
- Student's `AverageRating` is calculated from all approved reviews
- Student's `TotalReviews` counter is updated when reviews are approved/rejected
- Reviews can be moderated before appearing on student profile
- Students can respond to reviews to address feedback
- Public reviews are visible on student profiles; private reviews are for internal use only

---

## Summary

This platform uses **24 core models** organized into:
- **5 user/profile entities**: User, Student, Company, University, Department
- **3 skill entities**: Skill, StudentSkill (junction), ProjectRequiredSkill (junction)
- **2 project entities**: Project, Application
- **5 messaging entities**: Conversation, ConversationParticipant (junction), Message, Notification
- **2 team entities**: ProjectGroup, GroupMember (junction)
- **7 analytics/audit/achievement entities**: Certificate, ActivityLog, DashboardMetric, UserSettings, CompletedOpportunity, Payment, CompanyReview, StudentReview

All models follow **clean POCO architecture** with Fluent API configurations providing comprehensive database schema definitions, indexes, relationships, and constraints.

**Bidirectional Review System**: Both students and companies can review each other after completing opportunities, creating transparent accountability and helping future matches make informed decisions.
