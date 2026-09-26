# 💼 JobApplication.API

A clean-architecture **ASP.NET Core 9 Web API** for managing job postings and applications — built for two roles, **Recruiters** and **Candidates**, with JWT authentication, background job automation, and a CQRS-driven application core.

Recruiters post and manage jobs, review applications, and update their status. Candidates browse open roles, apply, track their applications, and withdraw them — all secured behind role-based authorization.

---

## 📖 Project Overview

**JobApplication.API** is the backend for a job board / recruitment platform. It models the core hiring workflow end-to-end:

- 🔐 Candidates and Recruiters register and authenticate via JWT
- 📋 Recruiters create, update, and close job postings
- 🎯 Candidates apply to open jobs and track their application status
- 🔄 Recruiters review applications and move them through a status pipeline (`Applied → UnderReview → Interview → Accepted / Rejected`)
- ⏰ Jobs automatically close themselves once their deadline passes, via a recurring background job
- 📣 Notifications are dispatched asynchronously whenever an application is created or cancelled

The project is built as a teaching/portfolio-grade reference implementation of **Clean Architecture + CQRS** in .NET — every layer has a single, well-defined responsibility.

---

## 🛠️ Tech Stack

| Category | Technology |
|---|---|
| **Framework** | ASP.NET Core 9 (Web API) |
| **Language** | C# 13 / .NET 9 |
| **Database** | SQL Server (via Entity Framework Core 9) |
| **Auth** | ASP.NET Core Identity + JWT Bearer Authentication |
| **CQRS / Mediator** | MediatR |
| **Object Mapping** | AutoMapper |
| **Background Jobs** | Hangfire (SQL Server storage) |
| **API Documentation** | Scalar (OpenAPI UI) via `Microsoft.AspNetCore.OpenApi` |
| **Architecture Style** | Clean Architecture (Domain / Application / Infrastructure / API) |

---

## 🏗️ Architecture

The solution follows **Clean Architecture**, split into four projects with a strict, one-directional dependency flow:

```
JobApplication.API  ──depends on──▶  Infrastructure  ──depends on──▶  Application  ──depends on──▶  Domain
```

- **`Domain`** — Pure business entities and enums (`Job`, `Candidate`, `Recruiter`, `JobApplication`, `JobApplicationStatus`). No dependencies on any other layer.
- **`Application`** — Business logic, organized by feature using **CQRS** (MediatR Commands & Queries), DTOs, AutoMapper profiles, custom exceptions, and service/repository *interfaces*. This layer has no knowledge of EF Core, ASP.NET, or SQL Server — it only depends on abstractions.
- **`Infrastructure`** — Concrete implementations of the Application layer's interfaces: EF Core `DbContext` and migrations, Identity (JWT issuing, `ApplicationUser`, role seeding), the Repository/Unit of Work pattern, Hangfire job scheduling, and notification services.
- **`JobApplication.API`** — The ASP.NET Core host: Controllers, middleware, DI composition root (`Program.cs`), and configuration.

**Key patterns used:**

- **CQRS via MediatR** — every use case is a `Command` or `Query` with its own dedicated `Handler`, keeping controllers thin (they just dispatch to `IMediator`).
- **Repository + Unit of Work** — a `IGenericRepository<T>` for generic CRUD plus specialized repositories (`IJobRepository`, `IJobApplicationRepository`), all coordinated through `IUnitOfWork` for atomic `SaveChangesAsync` calls.
- **Global exception handling** — a custom `ExceptionMiddleware` maps domain exceptions (`NotFoundException`, `BadRequestException`, `UnauthorizedException`, `ForbiddenException`, `ValidationException`) to the correct HTTP status codes, returning consistent JSON error responses.
- **`ICurrentUserService`** — resolves the authenticated user's identity, `CandidateId`, or `RecruiterId` from JWT claims, keeping handlers free of `HttpContext` access.
- **Recurring background jobs** — Hangfire runs a minutely job (`CloseExpiredJobsAsync`) that automatically closes any job posting past its `CloseAt` deadline.

---

## ✨ Features

**Authentication & Authorization**
- Register as a `Candidate` or `Recruiter` (candidates must supply a CV URL)
- Secure login issuing a signed JWT with role claims
- Role-based access control enforced on every endpoint (`[Authorize(Roles = "...")]`)

**Job Management** (Recruiter)
- Create job postings with a title, description, and optional auto-close date
- Update job details
- Manually close a job posting

**Job Discovery** (All authenticated users)
- Browse all jobs
- View job details by ID

**Applications** (Candidate)
- Apply to an open job (duplicate/cancelled-safe application checks included)
- View all of your own applications, or a single application by ID
- Cancel a pending application

**Applications** (Recruiter)
- View all applications submitted to a specific job posting
- Update an application's status through the hiring pipeline

**Automation**
- Jobs close themselves automatically once their `CloseAt` time passes (recurring Hangfire job, runs every minute)
- Recruiters are notified when a candidate applies; candidates are notified when they cancel

---

## 🧪 Testing

There is currently **no automated test suite** included in this repository.

Because the Application layer is built around MediatR handlers operating purely against interfaces (`IUnitOfWork`, `ICurrentUserService`, `IBackgroundJobScheduler`, etc.), it's well-suited for unit testing with mocking frameworks. Recommended next steps:

- Add an `Application.Tests` project (xUnit + Moq/NSubstitute) to unit test command/query handlers in isolation
- Add an `Infrastructure.Tests` / integration test project using the EF Core InMemory or SQLite provider to validate repository and `DbContext` behavior
- Add `WebApplicationFactory`-based integration tests for controller endpoints, including auth and role enforcement

---

## 📁 Folder Structure

```
JobApplication.API/
│
├── Domain/                          # Enterprise business rules — no external dependencies
│   ├── Entities/                    # Job, Candidate, Recruiter, JobApplication
│   └── Enums/                       # JobApplicationStatus
│
├── Application/                     # Use cases, orchestrated via CQRS
│   ├── DTOs/                        # Request/response data contracts
│   ├── Exceptions/                  # Custom domain exceptions (NotFound, BadRequest, etc.)
│   ├── Features/
│   │   ├── Jobs/
│   │   │   ├── Commands/            # CreateJob, UpdateJob, CloseJob
│   │   │   └── Queries/             # GetAllJobs, GetJobById
│   │   └── JobApplications/
│   │       ├── Commands/            # Apply, Cancel, UpdateStatus
│   │       └── Queries/             # GetJobApplications, GetMyApplications(ById)
│   ├── Interfaces/                  # Repository & service abstractions
│   ├── Mapping/                     # AutoMapper profiles
│   └── Services/                    # Application-level service implementations
│
├── Infrastructure/                  # External concerns: persistence, identity, jobs
│   ├── Identity/                    # ApplicationUser, JWT issuing, role seeding
│   ├── Migrations/                  # EF Core migrations
│   ├── Persistence/                 # ApplicationDbContext, entity configurations
│   ├── Repositories/                # Generic + specialized repositories, UnitOfWork
│   └── Services/                    # Hangfire scheduler, job maintenance, notifications
│
└── JobApplication.API/              # Composition root / host
    ├── Controllers/                 # AuthenticationController, JobsController, ApplicationsController
    ├── Middleware/                  # Global ExceptionMiddleware
    ├── Program.cs                   # DI setup, auth, Hangfire, OpenAPI configuration
    └── appsettings.json             # Connection strings, JWT settings
```

---

## 🚀 How to Run the Project

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- SQL Server (LocalDB, Express, or full instance)

### 1. Clone the repository

```bash
git clone https://github.com/<your-username>/JobApplication.API.git
cd JobApplication.API
```

### 2. Configure your settings

Update `JobApplication.API/appsettings.json` (or use `dotnet user-secrets`) with your own connection strings and JWT secret:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "YOUR_SQL_SERVER_CONNECTION_STRING",
    "HangfireConnection": "YOUR_HANGFIRE_CONNECTION_STRING"
  },
  "Jwt": {
    "Key": "YOUR_STRONG_SECRET_KEY",
    "Issuer": "JobApplication.API",
    "Audience": "JobApplication.Client",
    "ExpirationInMinutes": 60
  }
}
```

### 3. Apply database migrations

```bash
dotnet ef database update --project Infrastructure --startup-project JobApplication.API
```

### 4. Run the API

```bash
dotnet run --project JobApplication.API
```

### 5. Explore the API

- **Scalar API Reference (Dev only):** `https://localhost:7215/scalar`
- **Hangfire Dashboard:** `https://localhost:7215/hangfire`

> On startup, `Candidate` and `Recruiter` roles are seeded automatically — register a user with either role via `POST /api/Authentication/Register` to get started.

---

## 🔭 Future Improvements

- ✅ Add a full automated test suite (unit + integration)
- 📄 Add pagination, filtering, and sorting to job/application listing endpoints
- 📎 Support real file uploads for CVs instead of a raw `CvUrl` string
- ✉️ Replace the log-based `EmailNotificationService` with real email delivery (e.g. SendGrid, SMTP)
- 🧱 Introduce FluentValidation for request-level validation pipelines
- 📊 Add recruiter-facing analytics (application counts, time-to-fill, etc.)
- 🐳 Add Docker & docker-compose support for one-command local setup
- 🔁 Add refresh-token support to the JWT auth flow
- ⚙️ Add CI/CD (GitHub Actions) for build, test, and deployment automation

---

## 🔗 Social Links

- **GitHub:** [github.com/your-username](https://github.com/manarghareeb)
- **LinkedIn:** [linkedin.com/in/your-profile](https://linkedin.com/in/manar-ghareeb)
- **Portfolio:** [your-portfolio-site.com](https://manar-ghareeb-portfolio.vercel.app)
- **Email:** manarghareeb1973@gmail.com

---

<p align="center">Made with ❤️ using ASP.NET Core</p>
