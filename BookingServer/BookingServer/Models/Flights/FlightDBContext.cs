using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BookingServer.Models.Flights
{
    public partial class FlightDBContext : DbContext
    {
        public virtual DbSet<FlBooking> FlBooking { get; set; }
        public virtual DbSet<Flight> Flight { get; set; }
        public virtual DbSet<FlightDetail> FlightDetail { get; set; }

        public FlightDBContext(DbContextOptions<FlightDBContext> options) : base(options)
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
            modelBuilder.Entity<FlBooking>(entity =>
            {
                entity.HasKey(e => e.BookingId);

                entity.Property(e => e.BookDate).HasColumnType("date");

                entity.Property(e => e.PayType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Flight>(entity =>
            {
                entity.Property(e => e.Destination)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.Locale)
                    .IsRequired()
                    .HasColumnType("text");
            });

            modelBuilder.Entity<FlightDetail>(entity =>
            {
                entity.HasKey(e => e.FdetailId);

                entity.Property(e => e.FdetailId).HasColumnName("FDetailId");

                entity.Property(e => e.FlightStatus)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.PicDirectory)
                    .IsRequired()
                    .HasColumnType("text");

                entity.HasOne(d => d.Flight)
                    .WithMany(p => p.FlightDetail)
                    .HasForeignKey(d => d.FlightId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FlightDetail_Flight");
            });
        }
    }
}
