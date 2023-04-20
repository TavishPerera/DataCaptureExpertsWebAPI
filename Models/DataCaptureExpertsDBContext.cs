using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DataCaptureExpertsWebAPI.Models
{
    public partial class DataCaptureExpertsDBContext : DbContext
    {
        public DataCaptureExpertsDBContext()
        {
        }

        public DataCaptureExpertsDBContext(DbContextOptions<DataCaptureExpertsDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<Supplier> Suppliers { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

                //optionsBuilder.UseSqlServer("Server=TAVISH-LAPTOP;Database=DataCaptureExpertsDB;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.ToTable("Customer");

                entity.Property(e => e.UserId).ValueGeneratedNever();

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(30);

                entity.Property(e => e.FirstName).HasMaxLength(30);

                entity.Property(e => e.LastName).HasMaxLength(30);

                entity.Property(e => e.Username).HasMaxLength(30);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.Property(e => e.OrderId).ValueGeneratedNever();

                entity.Property(e => e.OrderedOn).HasColumnType("datetime");

                entity.Property(e => e.ShippedOn).HasColumnType("datetime");

                entity.HasOne(d => d.OrderByNavigation)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.OrderBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_Customer");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_Product");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");

                entity.Property(e => e.ProductId).ValueGeneratedNever();

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.ProductName).HasMaxLength(50);

                entity.Property(e => e.UnitPrice).HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.SupplierId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_Product");
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.ToTable("Supplier");

                entity.Property(e => e.SupplierId).ValueGeneratedNever();

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.SupplierName).HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
