# 📚 Bookstore Management System (EF Core & Dapper)

![.NET 10](https://img.shields.io/badge/.NET-10.0-blueviolet)
![EF Core](https://img.shields.io/badge/ORM-EF%20Core-blue)
![Dapper](https://img.shields.io/badge/Micro--ORM-Dapper-green)
![SQL Server](https://img.shields.io/badge/DB-SQL%20Server-red)

## 📖 Overview
A professional-grade console application designed to automate bookstore operations. This project demonstrates a hybrid data access strategy, leveraging **Entity Framework Core** for complex business logic and state management, alongside **Dapper** for high-performance read-only queries and analytical reporting.

## 🏗️ Architecture: Repository Pattern
To ensure scalability and maintainability, the project implements the **Repository Pattern**, achieving a clean **Separation of Concerns (SoC)**:
- **Domain Layer**: Contains entity models and repository interfaces.
- **Data Layer**: Manages the `DbContext`, Fluent API configurations, and repository implementations.
- **App Layer**: Orchestrates services and business scenarios within the `Program.cs` entry point.

---

## 🛠️ Technology Stack
- **Runtime:** .NET 10.0
- **Database:** SQL Server (LocalDB)
- **Data Access Layer:**
  - **EF Core:** Handles CRUD operations, Fluent API configurations, Migrations, and Eager Loading (`Include`/`ThenInclude`).
  - **Dapper:** Executes raw SQL for optimized reporting and lightweight data retrieval.
- **Logging:** Integrated EF Core logging to monitor generated SQL commands in real-time.

---

## 📊 Database Design (Fluent API)
The database schema is configured using **Fluent API** to enforce strict data integrity directly at the SQL Server level:

### Entities & Constraints:
- **Author**: Unique index on `FullName` (max length: 100).
- **Book**: 
  - `Price` — `decimal(10,2)` type with a Check Constraint: `Price > 0`.
  - `StockQuantity` — Check Constraint: `StockQuantity >= 0`.
  - Non-clustered index on `Title` for search optimization.
- **Order**: Automatic timestamp generation using `GETDATE()`.
- **OrderItem**: Composite Primary Key (`OrderId`, `BookId`) with a Check Constraint: `Quantity > 0`.

### Relationships:
- **One-to-Many**: One Author → Many Books. Configured with `DeleteBehavior.Restrict` to prevent accidental data loss.
- **Many-to-Many**: Orders ↔ Books via the `OrderItem` join table.

---

## 📂 Project Structure

```
BookstoreManagement/
├── Interfaces/
│   ├── IBookRepository.cs     # Contracts for Book-related operations
│   └── IOrderRepository.cs    # Contracts for Orders and Reporting
├── Data/
│   ├── BookShopDbContext.cs   # Context configuration and Fluent API
│   └── Repositories/          # Repository implementations (EF + Dapper)
├── Models/                    # Domain entity models
├── Migrations/                # EF Core migrations and schema history
└── Program.cs                 # Application entry point & orchestration
```

---

## 📝 Key Scenarios

### EF Core (Business Logic)

* **Create:** Add authors, books, and multi-item orders (transactional flows).
* **Read:** Fetch books with related authors using `.Include()` and `.ThenInclude()`.
* **Update / Delete:** Update prices and stock, safely remove orders after validation.

### Dapper (High-Speed Analytics)

* Retrieve lightweight Book DTOs (Id, Title, Price) for fast UI lists or reports.
* Complex multi-table join reports for analytics and exports.
