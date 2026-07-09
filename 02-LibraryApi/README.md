# Level 2: Library Loan System API

An ASP.NET Core 8 Web API for a **Library Loan System** implementing a strict Repository pattern with Clean Architecture. 

This project represents the **Medium** complexity level (Level 2) in the API series. It demonstrates authentication, role-based authorization, inventory limits, and automated calculations.

## Features

- **Standard Clean Architecture**: Decoupled projects (`Domain`, `Application`, `Infrastructure`, `API`).
- **Role-Based Auth (JWT)**: Supports **Admin** and **Librarian** roles.
- **Inventory Tracking**: Decrements/increments book stock copies dynamically when books are borrowed and returned.
- **Automated Fine Calculations**: Calculates late return fees (5,000 IDR per day after the DueDate) automatically upon return.
- **Switchable Databases**: Supports SQLite (default) and SQL Server.
- **Global Error Handling**: Translates DomainExceptions into proper REST HTTP status responses.

---

## Seed Accounts

| Account | Email | Password | Role |
|---------|-------|----------|------|
| System Admin | `admin@library.com` | `password123` | Admin |
| Librarian | `john@library.com` | `password123` | Librarian |

---

## Technical Specifications & Endpoints

### 1. Authentication (`/api/auth`)
- `POST /api/auth/register` — Create a new account.
- `POST /api/auth/login` — Login and receive a JWT.
- `GET /api/auth/me` (Protected) — View current profile.
- `GET /api/auth/librarians` (Admin Only) — Get all registered librarians.
- `DELETE /api/auth/librarians/{id}` (Admin Only) — Remove a librarian account.

### 2. Categories (`/api/categories`)
- `GET /api/categories` (Protected) — Get all categories.
- `GET /api/categories/{id}` (Protected) — Get category details.
- `POST /api/categories` (Admin Only) — Create a category.
- `PUT /api/categories/{id}` (Admin Only) — Update a category.
- `DELETE /api/categories/{id}` (Admin Only) — Delete category (restricted if books are linked).

### 3. Books (`/api/books`)
- `GET /api/books` (Protected) — Paginated book catalog with search.
- `GET /api/books/{id}` (Protected) — Get book details.
- `POST /api/books` (Protected) — Create a book.
- `PUT /api/books/{id}` (Protected) — Update book details.
- `DELETE /api/books/{id}` (Admin Only) — Delete book (restricted if active loans exist).

### 4. Borrowings (`/api/borrowings`)
- `GET /api/borrowings` (Protected) — View borrowings with status/search filters.
- `GET /api/borrowings/{id}` (Protected) — View specific borrowing details.
- `POST /api/borrowings` (Protected) — Borrow a book (reduces book availability stock).
- `PUT /api/borrowings/{id}/return` (Protected) — Return a book (re-stocks the book copy & calculates late return fines).
- `POST /api/borrowings/overdue/process` (Protected) — Cron-like process to mark overdue loans.

---

## How to Run

1. Open a terminal in this directory (`02-LibraryApi`).
2. Run the application:
   ```bash
   dotnet run --project src/LibraryApi.API/LibraryApi.API.csproj
   ```
3. Access the Swagger documentation:
   👉 **[http://localhost:5002/swagger](http://localhost:5002/swagger)**
