-- =============================================
-- ERPLite Database - Stored Procedures
-- Run these AFTER the app creates the tables
-- via EnsureCreated() on first launch
-- =============================================

USE ERPLiteDB;
GO

-- =============================================
-- SP 1: Department-wise Salary Report
-- =============================================
CREATE OR ALTER PROCEDURE usp_GetDepartmentSalaryReport
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        d.Name                      AS Department,
        COUNT(e.Id)                 AS TotalEmployees,
        SUM(e.Salary)               AS TotalSalary,
        AVG(e.Salary)               AS AverageSalary,
        MIN(e.Salary)               AS MinSalary,
        MAX(e.Salary)               AS MaxSalary
    FROM
        Departments d
        LEFT JOIN Employees e ON e.DepartmentId = d.Id AND e.IsActive = 1
    GROUP BY
        d.Name
    ORDER BY
        TotalSalary DESC;
END
GO

-- =============================================
-- SP 2: Search Employees (name / email / dept)
-- =============================================
CREATE OR ALTER PROCEDURE usp_SearchEmployees
    @SearchTerm     NVARCHAR(100) = NULL,
    @DepartmentId   INT           = NULL,
    @IsActive       BIT           = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        e.Id,
        e.FirstName + ' ' + e.LastName  AS FullName,
        e.Email,
        e.PhoneNumber,
        e.Designation,
        e.Salary,
        e.JoiningDate,
        e.IsActive,
        d.Name AS Department
    FROM
        Employees e
        INNER JOIN Departments d ON d.Id = e.DepartmentId
    WHERE
        (@SearchTerm IS NULL
            OR e.FirstName LIKE '%' + @SearchTerm + '%'
            OR e.LastName  LIKE '%' + @SearchTerm + '%'
            OR e.Email     LIKE '%' + @SearchTerm + '%')
        AND (@DepartmentId IS NULL OR e.DepartmentId = @DepartmentId)
        AND (@IsActive     IS NULL OR e.IsActive     = @IsActive)
    ORDER BY
        e.FirstName, e.LastName;
END
GO

-- =============================================
-- SP 3: Soft-delete employee (set IsActive = 0)
-- =============================================
CREATE OR ALTER PROCEDURE usp_DeactivateEmployee
    @EmployeeId INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Employees
    SET IsActive = 0
    WHERE Id = @EmployeeId;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

-- =============================================
-- SP 4: Get employee count per department
-- =============================================
CREATE OR ALTER PROCEDURE usp_GetHeadcount
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        d.Name          AS Department,
        COUNT(e.Id)     AS ActiveEmployees
    FROM
        Departments d
        LEFT JOIN Employees e ON e.DepartmentId = d.Id AND e.IsActive = 1
    GROUP BY
        d.Name
    ORDER BY
        ActiveEmployees DESC;
END
GO
