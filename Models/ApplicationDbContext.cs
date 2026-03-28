using Microsoft.EntityFrameworkCore;

namespace ERPLite.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<AppUser> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Employee -> Department relationship
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Unique email constraint
            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Email)
                .IsUnique();

            // Unique username constraint
            modelBuilder.Entity<AppUser>()
                .HasIndex(u => u.Username)
                .IsUnique();
        }
    }

    // Seed default data (departments + admin user)
    public static class DataSeeder
    {
        public static void Seed(ApplicationDbContext db)
        {
            // Seed departments
            if (!db.Departments.Any())
            {
                db.Departments.AddRange(
                    new Department { Name = "Human Resources", Description = "Manages hiring and employee relations" },
                    new Department { Name = "Finance", Description = "Handles accounts and budgeting" },
                    new Department { Name = "IT", Description = "Manages technology and infrastructure" },
                    new Department { Name = "Operations", Description = "Oversees day-to-day business operations" }
                );
                db.SaveChanges();
            }

            // Seed admin user (password: Admin@123)
            if (!db.Users.Any())
            {
                db.Users.Add(new AppUser
                {
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    Role = "Admin"
                });
                db.SaveChanges();
            }
        }
    }
}
