﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BookingServer.Models.Accommodations
{
    public partial class AccommodationDBContext : DbContext
    {
        public virtual DbSet<AccBooking> AccBooking { get; set; }
        public virtual DbSet<AccDetail> AccDetail { get; set; }
        public virtual DbSet<Accommodation> Accommodation { get; set; }
        public virtual DbSet<Property> Property { get; set; }

        public AccommodationDBContext(DbContextOptions<AccommodationDBContext> options)
            : base(options)
        {
            try
            {
                Database.EnsureCreated();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccBooking>(entity =>
            {
                entity.HasKey(e => e.BookingId);

                entity.Property(e => e.BookDate).HasColumnType("date");

                entity.Property(e => e.PayType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Detail)
                    .WithMany(p => p.AccBooking)
                    .HasForeignKey(d => d.DetailId)
                    .HasConstraintName("FK_AccBooking_AccDetail");
            });

            modelBuilder.Entity<AccDetail>(entity =>
            {
                entity.HasKey(e => e.DetailId);

                entity.Property(e => e.DateAvailableFrom).HasColumnType("date");

                entity.Property(e => e.DateAvailableTo).HasColumnType("date");

                entity.Property(e => e.PropertyAttr)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.RoomType)
                    .IsRequired()
                    .IsUnicode(false);

                entity.HasOne(d => d.Prop)
                    .WithMany(p => p.AccDetail)
                    .HasForeignKey(d => d.PropId)
                    .HasConstraintName("FK_AccDetail_Property");
            });

            modelBuilder.Entity<Accommodation>(entity =>
            {
                entity.HasKey(e => e.AccId);

                entity.Property(e => e.Country)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Location)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Picture).IsRequired();
            });

            modelBuilder.Entity<Property>(entity =>
            {
                entity.HasKey(e => e.PropId);

                entity.Property(e => e.Picture).IsRequired();

                entity.Property(e => e.PropName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Acc)
                    .WithMany(p => p.Property)
                    .HasForeignKey(d => d.AccId)
                    .HasConstraintName("FK_Property_Accommodation");
            });
        }
    }
}
