using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BookingServer.Models.AirTaxis
{
    public partial class AirTaxiDBContext : DbContext
    {
        public virtual DbSet<AirBooking> AirBooking { get; set; }
        public virtual DbSet<AirDetail> AirDetail { get; set; }
        public virtual DbSet<AirTaxiDropOff> AirTaxiDropOff { get; set; }
        public virtual DbSet<AirTaxiPickUp> AirTaxiPickUp { get; set; }
        public virtual DbSet<Taxi> Taxi { get; set; }

        public AirTaxiDBContext(DbContextOptions<AirTaxiDBContext> options)
                                                    : base(options)
        {
            try
            {
                Database.EnsureCreated();
            }
            catch (Exception ex)
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
            modelBuilder.Entity<AirBooking>(entity =>
            {
                entity.HasKey(e => e.BookingId);

                entity.Property(e => e.BookDate).HasColumnType("datetime");

                entity.Property(e => e.PayType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ReturnJourney).HasColumnType("datetime");

                entity.Property(e => e.TaxiName)
                    .IsRequired()
                    .IsUnicode(false)
                    .HasDefaultValueSql("('Taxi')");

                entity.HasOne(d => d.AirDetail)
                    .WithMany(p => p.AirBooking)
                    .HasForeignKey(d => d.AirDetailId)
                    .HasConstraintName("FK_AirBooking_AirDetail");
            });

            modelBuilder.Entity<AirDetail>(entity =>
            {
                entity.Property(e => e.DriverPolicy)
                    .IsRequired()
                    .IsUnicode(false);

                entity.HasOne(d => d.DropOff)
                    .WithMany(p => p.AirDetail)
                    .HasForeignKey(d => d.DropOffId)
                    .HasConstraintName("FK_AirDetail_AirTaxiDropOff");

                entity.HasOne(d => d.Taxi)
                    .WithMany(p => p.AirDetail)
                    .HasForeignKey(d => d.TaxiId)
                    .HasConstraintName("FK_AirDetail_Taxi");
            });

            modelBuilder.Entity<AirTaxiDropOff>(entity =>
            {
                entity.HasKey(e => e.DropOffId);

                entity.Property(e => e.DropOff)
                    .IsRequired()
                    .IsUnicode(false);

                entity.HasOne(d => d.PickUp)
                    .WithMany(p => p.AirTaxiDropOff)
                    .HasForeignKey(d => d.PickUpId)
                    .HasConstraintName("FK_AirTaxiDropOff_AirTaxiPickUp");
            });

            modelBuilder.Entity<AirTaxiPickUp>(entity =>
            {
                entity.HasKey(e => e.PickUpId);

                entity.Property(e => e.PickUp)
                    .IsRequired()
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Taxi>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .IsUnicode(false);
            });
        }
    }
}
