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
        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductLevel> ProductLevels { get; set; }
        public DbSet<Purchase> Purchases { get; set; }

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

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(500);
                //entity.Property(p => p.SKU).HasMaxLength(100);
                entity.Property(p => p.ImageUrl).HasColumnType("text");
                entity.Property(p => p.CreatedAt).IsRequired();
            });

            modelBuilder.Entity<Purchase>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.SKU).IsRequired().HasMaxLength(100);
                entity.Property(p => p.TotalAmount).HasColumnType("decimal(18,2)");
                entity.Property(p => p.CreatedAt).IsRequired();
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(200);
                entity.Property(c => c.Slug).HasMaxLength(200);
                entity.Property(c => c.CreatedAt).IsRequired();
                // store potentially large base64 data or image url
                entity.Property(c => c.ImageUrl).HasColumnType("text");
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

            modelBuilder.Entity<SubCategory>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Name).IsRequired().HasMaxLength(200);
                entity.Property(s => s.Slug).HasMaxLength(200);
                entity.Property(s => s.Code).HasMaxLength(100);
                entity.Property(s => s.CreatedAt).IsRequired();
                entity.HasIndex(s => s.Slug);
                entity.HasOne<Category>().WithMany().HasForeignKey(s => s.CategoryId).OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
