# ERPLite – Employee Management System

A lightweight ERP-style **Employee & Department Management System** built with **ASP.NET Core 8 MVC**, **Entity Framework Core**, and **Microsoft SQL Server**.

---

## Features

- **Authentication** – Cookie-based login with BCrypt password hashing
- **Employee CRUD** – Add, view, edit, delete employees with validation
- **Department CRUD** – Manage departments; prevents deletion if employees exist
- **Search & Filter** – Filter employees by name, email, department, or status
- **Salary Report** – Department-wise report powered by a SQL Server **stored procedure**
- **Dashboard** – Live stats: headcount, salary bill, recent joiners
- **Data Validation** – Server-side validation with duplicate email/name detection

---

## Tech Stack

| Layer       | Technology                        |
|-------------|-----------------------------------|
| Backend     | ASP.NET Core 8 MVC (C#)          |
| ORM         | Entity Framework Core 8           |
| Database    | Microsoft SQL Server              |
| Frontend    | Bootstrap 5, Bootstrap Icons      |
| Auth        | Cookie Authentication + BCrypt    |
| CI/DevOps   | GitHub / Git                      |

---

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express edition is fine)
- [SQL Server Management Studio](https://aka.ms/ssmsfullsetup) (optional)

### Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/ERPLite.git
   cd ERPLite
   ```

2. **Update connection string** in `appsettings.json`:
   ```json
   "DefaultConnection": "Server=YOUR_SERVER;Database=ERPLiteDB;Trusted_Connection=True;TrustServerCertificate=True;"
   ```

3. **Restore packages and run**
   ```bash
   dotnet restore
   dotnet run
   ```
   The app will auto-create the database and seed default departments + admin user on first launch.

4. **Create stored procedures** *(for the Salary Report feature)*
   Open `SQL/StoredProcedures.sql` in SSMS and execute against `ERPLiteDB`.

5. **Login**
   ```
   Username: admin
   Password: Admin@123
   ```

---

## Project Structure

```
ERPLite/
├── Controllers/
│   ├── AccountController.cs   # Login / Logout
│   ├── HomeController.cs      # Dashboard
│   ├── EmployeeController.cs  # Employee CRUD + Report
│   └── DepartmentController.cs
├── Models/
│   ├── Employee.cs
│   ├── Department.cs
│   ├── AppUser.cs
│   └── ApplicationDbContext.cs  # EF Core DbContext + Seeder
├── Views/
│   ├── Shared/_Layout.cshtml   # Sidebar layout
│   ├── Account/Login.cshtml
│   ├── Home/Index.cshtml       # Dashboard
│   ├── Employee/               # Index, Create, Edit, Details, Delete, Report
│   └── Department/             # Index, Create, Edit, Delete
├── SQL/
│   └── StoredProcedures.sql    # 4 stored procedures
├── appsettings.json
└── Program.cs
```

---

## Stored Procedures

| Procedure | Description |
|-----------|-------------|
| `usp_GetDepartmentSalaryReport` | Salary stats per department (used in Report page) |
| `usp_SearchEmployees` | Parameterised employee search |
| `usp_DeactivateEmployee` | Soft-delete (sets IsActive = 0) |
| `usp_GetHeadcount` | Active employee count per department |

---

## Screenshots

> Add screenshots here after running locally.

---

## Author

**Divyansh Chaurasia**  
MCA Student – IET Lucknow  
[GitHub](https://github.com/DivyanshCh39) | [LinkedIn](https://linkedin.com/in/divyanshch)
