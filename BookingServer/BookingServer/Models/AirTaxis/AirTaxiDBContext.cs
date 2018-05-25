using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BookingServer.Models.AirTaxis
{
    public partial class AirTaxiDBContext : DbContext
    {
        public virtual DbSet<AirBooking> AirBooking { get; set; }
        public virtual DbSet<AirTaxiDropOff> AirTaxiDropOff { get; set; }
        public virtual DbSet<AirTaxiPickUp> AirTaxiPickUp { get; set; }
        public virtual DbSet<Taxi> Taxi { get; set; }

        public AirTaxiDBContext(DbContextOptions<AirTaxiDBContext> options)
            :base(options)
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

                entity.Property(e => e.BookDate).HasColumnType("datetime");

                entity.Property(e => e.PayType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ReturnJourney).HasColumnType("datetime");

                entity.HasOne(d => d.DropOff)
                    .WithMany(p => p.AirBooking)
                    .HasForeignKey(d => d.DropOffId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AirBooking_TaxiDropOff");
            });

            modelBuilder.Entity<AirTaxiDropOff>(entity =>
            {
                entity.HasKey(e => e.DropOffId);

                entity.Property(e => e.DropOff)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.PickUp)
                    .WithMany(p => p.AirTaxiDropOff)
                    .HasForeignKey(d => d.PickUpId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TaxiDropOff_AirTaxiPickUp");
            });

            modelBuilder.Entity<AirTaxiPickUp>(entity =>
            {
                entity.HasKey(e => e.PickUpId);

                entity.Property(e => e.PickUp)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Taxi>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });
        }
    }
}
