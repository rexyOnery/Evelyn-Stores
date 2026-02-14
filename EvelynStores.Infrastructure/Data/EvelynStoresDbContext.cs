// Ensure you have installed the Microsoft.EntityFrameworkCore NuGet package.
// Run: dotnet add package Microsoft.EntityFrameworkCore

using System;
using System.Collections.Generic;
using System.Text;
using EvelynStores.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace EvelynStores.Infrastructure.Data
{
    public class EvelynStoresDbContext : DbContext
    {
        public EvelynStoresDbContext(DbContextOptions<EvelynStoresDbContext> options)
            : base(options)
        {
            // options parameter is used in base constructor, fixes CS9113
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(256);
                entity.Property(u => u.PasswordHash).IsRequired();
            });
        }
    }
}
