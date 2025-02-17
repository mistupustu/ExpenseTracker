﻿using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<ExpenseCategory> ExpenseCategories { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Unique fields
            modelBuilder.Entity<User>().
                HasIndex(u => u.Username).
                IsUnique();

            modelBuilder.Entity<User>().
                HasIndex(u => u.Email).
                IsUnique();

            modelBuilder.Entity<UserRole>().
                HasIndex(ur => ur.Name).
                IsUnique();

            modelBuilder.Entity<ExpenseCategory>().
                HasIndex(ec => ec.Name).
                IsUnique();

            //Relationships
            modelBuilder.Entity<Expense>().
                HasOne(e => e.ExpenseCategory).
                WithMany(ec => ec.Expenses).
                HasForeignKey(e => e.ExpenseCategoryId).
                OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Expense>().
                HasOne(e => e.User).
                WithMany(u => u.Expenses).
                HasForeignKey(e => e.UserId).
                OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>().
                HasOne(u => u.Role).
                WithMany(ur => ur.Users).
                HasForeignKey(u => u.UserRoleId).
                OnDelete(DeleteBehavior.SetNull);
        }
    }
}
