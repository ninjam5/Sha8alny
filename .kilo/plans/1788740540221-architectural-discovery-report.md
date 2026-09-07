# Sha8alny — Architectural Discovery Report & Code-Generation Constraints

> Verified against actual code (not docs) on 2026-09-07. Solution: `Sh8lnySolution.sln`, .NET 9, 7 projects.
> This report is the binding reference for all future code generation.

---

## 1. Architectural Pattern (Verified)

**Onion Architecture** with Repository + Unit of Work and a service-oriented application layer.
NOT Clean Architecture (no MediatR, no CQRS, no UseCase/Handler layer) and NOT classic N-Tier (dependencies invert via abstractions).

### Verified dependency graph (from .csproj files)

| Project | References | Role |
|---|---|---|
| `Sh8lny.Domain` | *(none)* | 33 POCO entities; enums co-located in entity files; NO BaseEntity; `required` props; nav props init `= null!` / `new HashSet<T>()` |
| `Sh8lny.Shared` | *(none)* | All DTOs (`DTOs/{Feature}/`), `Options/` (JwtOptions, MailSettings), `Validation/` (AllowedFileExtensionsAttribute) |
| `Sh8lny.Abstraction` | Domain, Shared (+ AspNetCore FrameworkReference) | `IGenericRepository<T>`, `IUnitOfWork`, `I{Feature}Service` interfaces |
| `Sh8lny.Service` | Abstraction, Domain (+ BCrypt, JWT, ImageSharp, nClam) | 20 business-logic service implementations |
| `Sh8lny.Persistence` | Abstraction, Domain, **Service** ⚠ | DbContext, GenericRepository, UnitOfWork, 31 Fluent configurations, 11 migrations, DbInitializer, MailService, BackupService |
| `Sh8lny.Presentation` | Abstraction | **Empty shell** (reserved) |
| `Sh8lny.Web` | All of the above | Composition root: 19 controllers, Program.cs (all DI), NotificationHub, SignalRNotifier, BackupWorker, MappingProfile, DiscordWebhookLoggerProvider |

⚠ **Documented deviation:** `Sh8lny.Persistence` → `Sh8lny.Service` reference exists (CONTEXT.md claims it doesn't). MailService and BackupService live in Persistence. **Do not add new Service→Persistence or Domain→anything references; treat the Persistence→Service edge as frozen legacy, not a pattern to extend.**

---

## 2. How Business Logic, Mapping, and Data Access Are Organized

### Business logic
- Lives exclusively in `Core/Sh8lny.Service/{Feature}Service.cs`, one service per feature.
- Services depend only on `IUnitOfWork`, other `I{Feature}Service` abstractions, and logged-in `userId` passed from controller.
- Authorization checks are **role-based at controller** (`[Authorize(Roles = "Company")]`) **plus ownership checks re-verified in the service** (e.g., `project.CompanyID != company.CompanyID` → failure).
- Transactions: `_unitOfWork.BeginTransactionAsync()` / `CommitTransactionAsync()` / `RollbackTransactionAsync()` wrapped in try/catch for multi-step writes.

### DTO mapping
- **Manual mapping is the real pattern**: private `static MapToResponseDto(...)` helpers inside services (see `ProjectService.cs:595`).
- AutoMapper IS registered (`AddAutoMapper(typeof(MappingProfile))` in Program.cs:86) and `MappingProfile.cs` exists, **but `IMapper` is never injected anywhere**. It is dead infrastructure — do NOT use IMapper in new code unless the user asks; follow the manual-mapper-helper convention.
- Enum → string via `.ToString()` in DTOs; DTOs expose `string` for enums on input, parsed with `Enum.TryParse<T>(ignoreCase: true)`.

### Database access
- Only via `IUnitOfWork` repository properties (`_unitOfWork.Projects.FindSingleAsync(...)`, etc.).
- **`GetQueryable()` does NOT exist** (CONTEXT.md Rule 2 is aspirational/stale). Available: `GetByIdAsync(int)`, `GetAllAsync()`, `FindAsync(pred)`, `FindSingleAsync(pred)`, `FindSingleAsync(pred, params includes)`, `AddAsync`, `AddRangeAsync`, `Update`, `Remove`, `RemoveRange`, `AnyAsync`, `CountAsync`.
- Service layer cannot use `.Include()` (no EF reference there). Complex reads = multiple repository round-trips + in-memory joins (e.g., `GetProjectByIdAsync` fetches project, then company, then skills separately; `GetFilteredProjectsAsync` loads all then filters in memory via `AsEnumerable()`).
- If a new query shape needs eager loading, prefer: dedicated method on `IUnitOfWork` (pattern exists: `GetStudentWithSkillsAsync`) implemented in `UnitOfWork` using `_context...Include(...)` — that keeps EF out of the Service layer.

---

## 3. Naming, Response Envelope, and Error Handling (Verified)

### Naming conventions
| Item | Convention | Example |
|---|---|---|
| Entity file/class | `{Entity}.cs` in `Domain/Models` | `Project.cs` |
| PK | `{Entity}ID` (exceptions: `ProjectModule.Id`, `ApplicationModuleProgress.Id`, `Transaction.Id`) | `ProjectID` |
| FK | `{Related}ID` | `CompanyID`, `UserID` |
| Enum | Co-located in entity file (no Enums folder) | `ProjectType` in `Project.cs` |
| Service / interface | `{Feature}Service` / `I{Feature}Service`; methods `Async` suffix | `CreateProjectAsync` |
| Controller | `{Feature}Controller`, `[ApiController]`, `[Route("api/[controller]")]` | `ProjectsController` |
| Sub-routes | kebab-case verbs | `my-projects`, `saved-projects`, `admin-review` |
| DTOs | `{Action}{Feature}Dto` in `Sh8lny.Shared/DTOs/{Feature}/` | `CreateProjectDto` |
| EF config | `{Entity}Configuration : IEntityTypeConfiguration<T>` in `Persistence/Configurations` | `ProjectConfiguration` |
| Namespaces | `Sh8lny.{Layer}.*`; **file-scoped** (`namespace X;`) in Service/Web/Shared, **block-scoped** in Domain/Persistence | — |
| Route casing | URLs resolve lowercase (`api/projects`) | — |

### Response envelope
```csharp
ServiceResponse<T> { bool IsSuccess; T? Data; string? Message; List<string> Errors }
// factories: ServiceResponse<T>.Success(data, message?) / .Failure(message, errors?)
PagedResult<T> { Items, PageNumber, PageSize, TotalCount, TotalPages, HasPreviousPage, HasNextPage }
```
- `PagedResult<T>.Create(items, pageNumber, pageSize, totalCount)` factory for pagination.

### Controller → HTTP status mapping (uniform pattern)
- `userId == null` → `401 Unauthorized(ServiceResponse<T>.Failure("Invalid or missing user token."))`
- `!result.IsSuccess` on read-by-id → `404 NotFound(result)`; on mutations → `400 BadRequest(result)`
- Success → `200 Ok(result)`; create → `201 CreatedAtAction(nameof(GetById), new { id }, result)`
- Controllers are THIN: extract `GetCurrentUserId()` (private helper, `ClaimTypes.NameIdentifier` → int), call exactly one service method, translate envelope. No business logic in controllers.

### Error handling
- **No global exception-handling middleware.** Only: inline request-timing middleware in Program.cs, Discord webhook logger provider.
- Services wrap logic in try/catch → `ServiceResponse<T>.Failure("An error occurred while ...", new List<string> { ex.Message })`, rolling back the transaction first if one is open.
- SignalR/Notifier failures must never throw (log & continue). Background worker failures must never crash the host.
- Validation: DataAnnotations (`[Required]`, `[MaxLength]`, `[StringLength]`) on newer Shared DTOs + manual in-service validation (deadline future, date order, enum parse). No FluentValidation. `required` C# keyword used on entity/DTO string props.

### Auth & infra facts
- JWT Bearer; claims: `NameIdentifier` (UserID), `Email`, `Role` (UserType). SignalR reads `access_token` from query string for `/hubs` paths.
- Auto-migrate + `DbInitializer.SeedAsync` at startup.
- DI: everything `Scoped` (repos, UoW, services, INotifier); `ILoggerProvider` singleton; `BackupWorker` hosted.
- EF enums stored as strings (`HasConversion<string>()`), defaults via `HasDefaultValueSql("GETDATE()")`, named indexes (`UQ_`/`IDX_` prefixes), `DeleteBehavior.Restrict` on FKs.
- 11 migrations, latest `20260605022039_AddAnnouncements`. The "pending" ones in CONTEXT.md (AddFcmTokenToUser, AddAppConfig) are applied.
- Tests project is empty (bin/obj only). No test framework in use.
- File uploads exclusively via `/api/Media`; everything else stores URL strings. NEVER `IFormFile` outside MediaController.

### Contradictions vs CONTEXT.md (code wins)
1. `ProjectStatus` in code = `Draft, Active, Pending, Complete, Cancelled, Closed` (docs claim `Open/InProgress/Closed/Completed`).
2. `GetQueryable()`/`.Include()` guidance in docs is not implemented; real pattern is repo round-trips + UoW helper methods.
3. Persistence→Service reference exists despite docs forbidding it.
4. AutoMapper effectively unused (manual mapping is the norm).

---

## 4. Strict Constraints I Will Enforce on All Future Code Generation

1. **Layer dependencies:** Domain/Shared stay dependency-free. Abstraction → Domain+Shared only. Service → Abstraction+Domain (+Shared transitively) only — NEVER Persistence/Web/EF. Persistence → Abstraction+Domain only (do not grow its Service reference; new infra services still get interface in Abstraction). Web is the only composition root.
2. **No EF Core types outside Persistence/Web:** Services use `IUnitOfWork` only. Need eager loading → add a named method to `IUnitOfWork` (e.g., `GetProjectWithModulesAsync`) implemented with `.Include()` inside `UnitOfWork`. Never LINQ-to-objects over `GetAllAsync()` for hot paths without checking with user first (it is the existing pattern, but flag scalability).
3. **Entities:** POCO in `Domain/Models/{Entity}.cs`, enum co-located, PK `{Entity}ID`, FK `{Related}ID`, `CreatedAt`/`UpdatedAt` maintained (`DateTime.UtcNow`), no `IFormFile`/HTTP/EF types. Follow exception list for `Id`-named PKs when extending those entities.
4. **New entity checklist:** Model → `{Entity}Configuration` (Fluent API, `ToTable`, `HasKey`, string-converted enums, `UQ_`/`IDX_` index names, `Restrict` deletes, `GETDATE()` defaults) → `DbSet` in `Sha8lnyDbContext` → repo property on `IUnitOfWork` + `UnitOfWork` (lazy `??=` pattern) → DTOs in `Sh8lny.Shared/DTOs/{Feature}/` → `I{Feature}Service` in Abstraction → `{Feature}Service` in Service → DI registration in `Program.cs` → controller in Web → **instruct developer to run `dotnet ef migrations add <Name> --startup-project ../Sh8lny.Web`** (never hand-write migrations; warn on data loss).
5. **Service methods:** signature `Task<ServiceResponse<T>> MethodAsync(int userId, ...)`; try/catch → `Failure("An error occurred while ...", new List<string> { ex.Message })`; business validation returns `Failure` with user-friendly message; ownership re-check inside service; multi-write flows wrapped in UoW transaction with rollback in catch.
6. **Controllers:** thin; `[ApiController]`, `[Route("api/[controller]")]`; role via `[Authorize(Roles = "...")]` / `[AllowAnonymous]`; private `GetCurrentUserId()` helper (NameIdentifier → int, null-safe); status mapping per Section 3; `CreatedAtAction` on creates; kebab-case sub-routes; XML doc comments on actions.
7. **DTOs:** in `Sh8lny.Shared/DTOs/{Feature}/`, named `{Action}{Feature}Dto`; DataAnnotations for shape validation (Required/MaxLength/StringLength with ErrorMessage); enums as `string?` with `Enum.TryParse(ignoreCase: true)` in service; pagination via `PagedResult<T>.Create`; never expose entities directly.
8. **Mapping:** manual private static `MapToResponseDto` helpers in services. Do not inject `IMapper` (not used anywhere despite registration).
9. **Never break runtime behaviors:** SignalR failures logged not thrown; BackupWorker never crashes host; startup auto-migration/seeding preserved; middleware order preserved (Swagger → HTTPS redirect (non-dev) → StaticFiles → timing middleware → CORS "AllowAll" → Auth → AuthZ → controllers → hubs).
10. **Config/secrets:** via `IOptions<T>` POCOs in `Sh8lny.Shared/Options` bound from `appsettings.json` / env vars; no hardcoded secrets.
11. **No new frameworks/patterns without asking:** no MediatR/CQRS, no FluentValidation, no global exception middleware, no soft deletes, no repository `GetQueryable` — the codebase doesn't have them today.
12. **Style:** file-scoped namespaces in Service/Web/Shared, block-scoped in Domain/Persistence (match neighbors); XML `///` docs on public service/controller members; `is null` / `is not null` pattern; `required` for non-nullable strings in entities/DTOs where established.
13. **Docs vs code conflicts:** code is truth; report contradictions rather than silently following CONTEXT.md.

---

## 5. Open Items (for user, not blocking)
- CONTEXT.md is stale in 4 places (Section 3 above) — recommend updating it before next milestone.
- `Sh8lny.Presentation` project is an empty shell — confirm whether to keep or remove from new work.
- Tests project is empty — any test framework choice needs user decision.
