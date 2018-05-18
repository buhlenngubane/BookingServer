using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BookingServer.Models.Flights
{
    public partial class FlightDBContext : DbContext
    {
        public virtual DbSet<AirFlight> AirFlight { get; set; }
        public virtual DbSet<Detail> Detail { get; set; }
        public virtual DbSet<FlBooking> FlBooking { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer(@"Data Source=S11;Initial Catalog=FlightDB;Integrated Security=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AirFlight>(entity =>
            {
                entity.HasKey(e => e.FlightId);

                entity.Property(e => e.Destination)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FlightType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Locale)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Detail>(entity =>
            {
                entity.HasKey(e => e.FdetailId);

                entity.Property(e => e.FdetailId).HasColumnName("FDetailId");

                entity.Property(e => e.Company)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Departure).HasColumnType("date");

                entity.Property(e => e.Path)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Picture).IsRequired();

                entity.Property(e => e.ReturnTrip).HasColumnType("date");

                entity.HasOne(d => d.Flight)
                    .WithMany(p => p.Detail)
                    .HasForeignKey(d => d.FlightId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Detail_AirFlight1");
            });

            modelBuilder.Entity<FlBooking>(entity =>
            {
                entity.HasKey(e => e.BookId);

                entity.Property(e => e.BookDate).HasColumnType("date");

                entity.Property(e => e.PayType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });
        }
    }
}
