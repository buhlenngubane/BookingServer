using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BookingServer.Models.CarRentals
{
    public partial class CarRentalDBContext : DbContext
    {
        public virtual DbSet<Car> Car { get; set; }
        public virtual DbSet<CarBooking> CarBooking { get; set; }
        public virtual DbSet<CarRental> CarRental { get; set; }
        public virtual DbSet<Company> Company { get; set; }

        public CarRentalDBContext(DbContextOptions<CarRentalDBContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Car>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PicDirectory)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CarBooking>(entity =>
            {
                entity.HasKey(e => e.BookingId);

                entity.Property(e => e.CrentId).HasColumnName("CRentId");

                entity.Property(e => e.PayType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Crent)
                    .WithMany(p => p.CarBooking)
                    .HasForeignKey(d => d.CrentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CarBooking_CarRental");
            });

            modelBuilder.Entity<CarRental>(entity =>
            {
                entity.HasKey(e => e.CrentId);

                entity.Property(e => e.CrentId).HasColumnName("CRentId");

                entity.Property(e => e.DropOff)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Location)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PickUp)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasKey(e => e.CmpId);

                entity.Property(e => e.CompanyName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CrentId).HasColumnName("CRentId");

                entity.Property(e => e.Routine)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Crent)
                    .WithMany(p => p.Company)
                    .HasForeignKey(d => d.CrentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Company_CarRental");
            });
        }
    }
}
