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
        public virtual DbSet<CarType> CarType { get; set; }
        public virtual DbSet<Ccompany> Ccompany { get; set; }

        public CarRentalDBContext(DbContextOptions<CarRentalDBContext> options)
                    : base(options)
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
                entity.Property(e => e.CtypeId).HasColumnName("CTypeId");

                entity.HasOne(d => d.Cmp)
                    .WithMany(p => p.Car)
                    .HasForeignKey(d => d.CmpId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Car_CCompany");

                entity.HasOne(d => d.Ctype)
                    .WithMany(p => p.Car)
                    .HasForeignKey(d => d.CtypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Car_CarType");
            });

            modelBuilder.Entity<CarBooking>(entity =>
            {
                entity.HasKey(e => e.BookingId);

                entity.Property(e => e.BookDate).HasColumnType("datetime");

                entity.Property(e => e.PayType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ReturnDate).HasColumnType("datetime");

                entity.HasOne(d => d.Car)
                    .WithMany(p => p.CarBooking)
                    .HasForeignKey(d => d.CarId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CarBooking_Car");
            });

            modelBuilder.Entity<CarRental>(entity =>
            {
                entity.HasKey(e => e.CrentId);

                entity.Property(e => e.CrentId).HasColumnName("CRentId");

                entity.Property(e => e.Location)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.PhysicalAddress).IsUnicode(false);
            });

            modelBuilder.Entity<CarType>(entity =>
            {
                entity.HasKey(e => e.CtypeId);

                entity.Property(e => e.CtypeId).HasColumnName("CTypeId");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Picture).IsRequired();

                entity.Property(e => e.Transmission)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Ccompany>(entity =>
            {
                entity.HasKey(e => e.CmpId);

                entity.ToTable("CCompany");

                entity.Property(e => e.CompanyName)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.CrentId).HasColumnName("CRentId");

                entity.Property(e => e.FuelPolicy)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Mileage)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Picture).IsRequired();

                entity.HasOne(d => d.Crent)
                    .WithMany(p => p.Ccompany)
                    .HasForeignKey(d => d.CrentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CCompany_CarRental");
            });
        }
    }
}
