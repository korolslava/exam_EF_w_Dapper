# 📚 Bookstore Management System (EF Core & Dapper)

![.NET 10](https://img.shields.io/badge/.NET-10.0-blueviolet)
![EF Core](https://img.shields.io/badge/ORM-EF%20Core-blue)
![Dapper](https://img.shields.io/badge/Micro--ORM-Dapper-green)
![SQL Server](https://img.shields.io/badge/DB-SQL%20Server-red)
![xUnit](https://img.shields.io/badge/Tests-xUnit-brightgreen)
![Docker](https://img.shields.io/badge/Container-Docker-2496ED)

## 📖 Overview

A professional-grade console application designed to automate bookstore operations. This project demonstrates a production-ready architecture with a **Service Layer**, **Dependency Injection**, **FluentValidation**, and a hybrid data access strategy — leveraging **Entity Framework Core** for complex business logic and **Dapper** for high-performance analytical queries.

The project is fully containerized with **Docker** and includes a comprehensive **unit test suite** (21 tests) to validate business logic in isolation.

---

## 🏗️ Architecture

The project follows a clean layered architecture with strict **Separation of Concerns**:

```
Program.cs  →  Service Layer  →  Repository Layer  →  EF Core / Dapper  →  SQL Server
```

- **Models** — Domain entity classes (`Book`, `Author`, `Order`, `OrderItem`)
- **DTOs** — Data Transfer Objects decoupled from persistence models (`BookDto`, `CreateBookDto`, `CreateOrderDto`, `OrderReportDto`)
- **Interfaces** — Contracts for repositories and services (`IBookRepository`, `IBookService`, etc.)
- **Repositories** — Data access implementations using EF Core and Dapper
- **Services** — Business logic layer (`BookService`, `OrderService`)
- **Validators** — Input validation via FluentValidation
- **Tests** — Isolated unit tests using xUnit, Moq, and FluentAssertions

---

## 🛠️ Technology Stack

| Layer | Technology |
|---|---|
| Runtime | .NET 10.0 |
| Database | SQL Server 2022 |
| ORM | Entity Framework Core 10 |
| Micro-ORM | Dapper |
| DI Container | Microsoft.Extensions.DependencyInjection |
| Validation | FluentValidation |
| Logging | Microsoft.Extensions.Logging + Console |
| Testing | xUnit v3, Moq, FluentAssertions |
| Containerization | Docker + docker-compose |

---

## 📊 Database Design (Fluent API)

The schema is configured via **Fluent API** to enforce data integrity at the SQL Server level:

### Entities & Constraints

- **Author** — Unique index on `FullName` (max 100 chars)
- **Book** — `decimal(10,2)` price with `CHECK (Price > 0)`, `CHECK (StockQuantity >= 0)`, non-clustered index on `Title`
- **Order** — Auto timestamp via `GETDATE()`
- **OrderItem** — Composite PK (`OrderId`, `BookId`) with `CHECK (Quantity > 0)`

### Relationships

- **One-to-Many**: Author → Books with `DeleteBehavior.Restrict`
- **Many-to-Many**: Orders ↔ Books via `OrderItem` join table

---

## 📂 Project Structure

```
BookstoreManagement/
├── Data/
│   ├── BookShopDbContext.cs          # DbContext with Fluent API configuration
│   └── DesignTimeDbContextFactory.cs # Factory for EF CLI migrations
├── DTOs/
│   ├── BookDto.cs
│   ├── CreateBookDto.cs
│   ├── CreateOrderDto.cs
│   └── OrderReportDto.cs
├── Interfaces/
│   ├── IBookRepository.cs
│   └── IOrderRepository.cs
├── Migrations/                        # EF Core migration history
├── Models/
│   ├── Author.cs
│   ├── Book.cs
│   ├── Order.cs
│   └── OrderItem.cs
├── Repositories/
│   ├── BookRepository.cs             # EF Core + Dapper hybrid
│   └── OrderRepository.cs           # EF Core + Dapper hybrid
├── Services/
│   ├── IBookService.cs
│   ├── BookService.cs
│   ├── IOrderService.cs
│   └── OrderService.cs
├── Validators/
│   ├── CreateBookDtoValidator.cs
│   └── CreateOrderDtoValidator.cs
├── appsettings.Development.json       # Local connection string (not in git)
├── appsettings.Docker.json            # Docker connection string
├── Dockerfile
└── Program.cs                         # DI setup + demo scenarios

BookstoreManagement.Tests/
├── BookServiceTests.cs               # 11 unit tests for BookService
└── OrderServiceTests.cs             # 10 unit tests for OrderService
```

---

## 🧪 Unit Tests (21 tests)

Tests are fully isolated — repositories are mocked with **Moq**, assertions use **FluentAssertions**.

**BookService (11 tests):**
- Create book with new author
- Reuse existing author instead of creating duplicate
- Reject negative / zero price
- Reject negative stock
- Verify `AddBookAsync` called exactly once
- Update price — valid and invalid cases
- Delete book with active orders → blocked
- Delete book with no orders → succeeds

**OrderService (10 tests):**
- Place order → correct email returned
- Place order → stock decremented
- Insufficient stock → exception
- Non-existent book → exception
- Empty email / empty items / zero quantity → validation exceptions
- Cancel order → stock restored
- Cancel non-existent order → exception
- Cancel order → `DeleteOrderAsync` called once

Run tests:
```bash
cd BookstoreManagement.Tests
dotnet run
```

---

## 📝 Key Scenarios (Program.cs)

### EF Core (Business Logic via Service Layer)
- **Scenario 1** — Create a book (author dedup logic, FluentValidation, stock check)
- **Scenario 2** — Retrieve all books with eager loading (`Include` / `ThenInclude`)
- **Scenario 4** — Place a transactional order (stock decrement, rollback on failure)

### Dapper (High-Speed Analytics)
- **Scenario 3** — Lightweight book list with author name (raw SQL join)
- **Scenario 5** — Order analytics report with `SUM(Quantity * Price)` aggregation

---

## 🐳 Running with Docker

No local SQL Server required. Docker Compose spins up SQL Server 2022 and the app together:

```bash
docker-compose up --build
```

The app automatically applies EF Core migrations on startup (`db.Database.Migrate()`), creates the database, and runs all demo scenarios.

---

## 🚀 Running Locally

1. Update `appsettings.Development.json` with your local SQL Server connection string
2. Apply migrations:
```bash
dotnet ef database update
```
3. Run the app:
```bash
dotnet run
```
