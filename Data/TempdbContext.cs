using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace mvc_obj_2.Data;

public partial class TempdbContext : DbContext
{
    public TempdbContext(DbContextOptions<TempdbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Item>? Items { get; set; }

    public virtual DbSet<Order>? Orders { get; set; }

    public virtual DbSet<OrdersItem>? OrdersItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Items__3214EC07D9E3DBAD");

            entity.Property(e => e.ItemName)
                .HasMaxLength(50)
                .HasColumnName("itemName");
            entity.Property(e => e.ItemPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("itemPrice");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Orders__3214EC0754D12669");

            entity.Property(e => e.OrderAddress)
                .HasMaxLength(100)
                .HasColumnName("orderAddress");
            entity.Property(e => e.OrderName)
                .HasMaxLength(50)
                .HasColumnName("orderName");
            entity.Property(e => e.Subtotal)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("subtotal");
        });

        modelBuilder.Entity<OrdersItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrdersIt__3214EC077812DFD0");

            entity.HasOne(d => d.Item).WithMany(p => p.OrdersItems)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("FK__OrdersIte__ItemI__3F466844");

            entity.HasOne(d => d.Order).WithMany(p => p.OrdersItems)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__OrdersIte__Order__3E52440B");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
