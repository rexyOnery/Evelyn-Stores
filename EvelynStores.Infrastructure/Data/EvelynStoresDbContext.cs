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
        public DbSet<PasswordResetOtp> PasswordResetOtps { get; set; }
        public DbSet<Unit> Units { get; set; }

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

            modelBuilder.Entity<PasswordResetOtp>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.Property(o => o.OtpCode).IsRequired().HasMaxLength(4);
                entity.Property(o => o.ResetToken).HasMaxLength(256);
                entity.HasOne(o => o.User)
                    .WithMany()
                    .HasForeignKey(o => o.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Unit>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Name).IsRequired().HasMaxLength(200);
                entity.Property(u => u.ShortName).HasMaxLength(50);
                entity.Property(u => u.CreatedAt).IsRequired();
            });
        }
    }
}
