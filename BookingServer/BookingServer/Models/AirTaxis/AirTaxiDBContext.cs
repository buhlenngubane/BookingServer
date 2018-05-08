using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BookingServer.Models.AirTaxis
{
    public partial class AirTaxiDBContext : DbContext
    {
        public virtual DbSet<AirBooking> AirBooking { get; set; }
        public virtual DbSet<AirTaxi> AirTaxi { get; set; }
        public virtual DbSet<Taxi> Taxi { get; set; }

        public AirTaxiDBContext(DbContextOptions<AirTaxiDBContext> options) :
            base()
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
            modelBuilder.Entity<AirBooking>(entity =>
            {
                entity.HasKey(e => e.BookingId);

                entity.Property(e => e.BookDate).HasColumnType("date");

                entity.Property(e => e.PayType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Air)
                    .WithMany(p => p.AirBooking)
                    .HasForeignKey(d => d.AirId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AirBooking_AirTaxi");
            });

            modelBuilder.Entity<AirTaxi>(entity =>
            {
                entity.HasKey(e => e.AirId);

                entity.Property(e => e.DropOff).HasColumnType("date");

                entity.Property(e => e.PickUp).HasColumnType("date");
            });

            modelBuilder.Entity<Taxi>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PicDirectory)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Air)
                    .WithMany(p => p.Taxi)
                    .HasForeignKey(d => d.AirId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Taxi_AirTaxi");
            });
        }
    }
}
