# .NET Clean Architecture API Examples

A collection of **three independent ASP.NET Core 8 Web API projects** built with Clean Architecture, designed as progressive learning references from beginner to advanced.

Each project can be run **individually** or **all together** — they are fully independent of each other.

---

## Projects at a Glance

| Level | Project | Domain | Entities | Auth Roles | Port |
|-------|---------|--------|----------|------------|------|
| 🟢 **Easy** | `01-BookstoreApi` | Book catalog | 3 | Authenticated (no roles) | 5001 |
| 🟡 **Medium** | `02-LibraryApi` | Library loan system | 4 | Admin, Librarian | 5002 |
| 🔴 **Hard** | `03-HotelApi` | Hotel reservation system | 6 | Admin, Receptionist, Guest | 5003 |

Each project progressively introduces more complexity:
- **BookstoreApi** — Architecture scaffold, basic JWT, CRUD, pagination, search
- **LibraryApi** — Role-based auth, soft delete, business rules, service interfaces
- **HotelApi** — State machine, date-range queries, ownership guards, PATCH, reports

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- (Optional) [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or SQL Server LocalDB — projects default to **SQLite** (no install needed)
- [Postman](https://www.postman.com/) or any API client for testing

---

## Quick Start — Run a Single Project

Each project is a self-contained solution. Pick the one you want to study:

```powershell
# Level 1 — BookstoreApi
cd 01-BookstoreApi/src/BookstoreApi.API
dotnet run
# → Swagger: http://localhost:5001/swagger

# Level 2 — LibraryApi
cd 02-LibraryApi/src/LibraryApi.API
dotnet run
# → Swagger: http://localhost:5002/swagger

# Level 3 — HotelApi
cd 03-HotelApi/src/HotelApi.API
dotnet run
# → Swagger: http://localhost:5003/swagger
```

On first run, the database is created automatically with seed data. No setup needed.

---

## Run All Projects Together

Use the provided script to start all three APIs in parallel:

```powershell
# Windows (PowerShell)
.\run-all.ps1
```

```bash
# Linux / macOS
chmod +x run-all.sh
./run-all.sh
```

All three APIs will be available simultaneously:

| API | URL |
|-----|-----|
| BookstoreApi | http://localhost:5001/swagger |
| LibraryApi | http://localhost:5002/swagger |
| HotelApi | http://localhost:5003/swagger |

To stop all jobs:
```powershell
Get-Job | Stop-Job | Remove-Job
```

---

## Switching the Database

Each project defaults to **SQLite** for zero-config local development. To switch to **SQL Server**, edit `appsettings.json` in the API project:

```json
{
  "DatabaseProvider": "SqlServer",
  "ConnectionStrings": {
    "SqlServer": "Server=localhost;Database=BookstoreDb;Trusted_Connection=True;TrustServerCertificate=True;",
    "Sqlite": "Data Source=bookstore.db"
  }
}
```

Change `DatabaseProvider` from `"Sqlite"` to `"SqlServer"` and update the connection string. Then run migrations:

```powershell
cd src/BookstoreApi.Infrastructure
dotnet ef database update --startup-project ../BookstoreApi.API
```

---

## Repository Structure

```
dotnet-api-example/
│
├── run-all.ps1               # Start all 3 APIs (PowerShell)
├── run-all.sh                # Start all 3 APIs (Bash)
│
├── 01-BookstoreApi/          # 🟢 Level 1 — Easy
│   ├── README.md             # Project overview & endpoints
│   ├── BookstoreApi.sln
│   ├── src/
│   │   ├── BookstoreApi.Domain/
│   │   ├── BookstoreApi.Application/
│   │   ├── BookstoreApi.Infrastructure/
│   │   └── BookstoreApi.API/
│   └── docs/
│       ├── endpoints.md
│       ├── database-schema.md
│       └── postman-collection.json
│
├── 02-LibraryApi/            # 🟡 Level 2 — Medium
│   ├── README.md
│   ├── LibraryApi.sln
│   ├── src/
│   │   ├── LibraryApi.Domain/
│   │   ├── LibraryApi.Application/
│   │   ├── LibraryApi.Infrastructure/
│   │   └── LibraryApi.API/
│   └── docs/
│       ├── endpoints.md
│       ├── database-schema.md
│       └── postman-collection.json
│
└── 03-HotelApi/              # 🔴 Level 3 — Hard
    ├── README.md
    ├── HotelApi.sln
    ├── src/
    │   ├── HotelApi.Domain/
    │   ├── HotelApi.Application/
    │   ├── HotelApi.Infrastructure/
    │   └── HotelApi.API/
    └── docs/
        ├── endpoints.md
        ├── database-schema.md
        └── postman-collection.json
```

---

## Architecture

All three projects follow the same **Pragmatic Clean Architecture** pattern:

```
Domain Layer          — Entities, Enums, Custom Exceptions
    ↑
Application Layer     — Services, DTOs, Validators, ApiResponse<T>
    ↑
Infrastructure Layer  — AppDbContext, EF Migrations, JwtService
    ↑
API Layer             — Controllers, GlobalExceptionHandler, Program.cs
```

**Key conventions shared across all projects:**

- All endpoints return a consistent `ApiResponse<T>` envelope
- All errors are handled by `GlobalExceptionHandler` middleware
- JWT authentication is required where noted — use `/api/auth/login` to get a token
- Database provider is switchable via `appsettings.json` (no code changes needed)

See each project's `README.md` for specific endpoint documentation.

---

## Testing with Swagger

1. Run a project (`dotnet run`)
2. Open `http://localhost:{port}/swagger`
3. Call `POST /api/auth/register` to create a user
4. Call `POST /api/auth/login` to get a JWT token
5. Click the **Authorize** button (🔒) at the top of Swagger
6. Enter: `Bearer <your-token>`
7. All protected endpoints are now accessible

---

## How to Read This Repo

**Start with `01-BookstoreApi`** if you are new to Clean Architecture. It has the simplest domain and the fewest moving parts — you can see the entire architecture without domain complexity getting in the way.

**Move to `02-LibraryApi`** once you understand the structure. It adds role-based auth, business rules, and a richer entity model.

**Study `03-HotelApi`** last. It is the most realistic project — closest to what you would encounter in a real time-pressured scenario — with date-range availability queries, a booking state machine, and fine-grained role permissions.

---

## Tech Stack

| Component | Technology |
|-----------|------------|
| Framework | ASP.NET Core 8 Web API |
| ORM | Entity Framework Core 8 |
| Default Database | SQLite |
| Optional Database | SQL Server / LocalDB |
| Validation | FluentValidation |
| Authentication | JWT Bearer |
| API Documentation | Swagger / Swashbuckle |
| Password Hashing | BCrypt.Net-Next |
