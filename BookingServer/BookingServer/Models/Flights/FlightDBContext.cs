using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BookingServer.Models.Flights
{
    public partial class FlightDBContext : DbContext
    {
        public virtual DbSet<Destination> Destination { get; set; }
        public virtual DbSet<FlBooking> FlBooking { get; set; }
        public virtual DbSet<FlCompany> FlCompany { get; set; }
        public virtual DbSet<Flight> Flight { get; set; }
        public virtual DbSet<FlightDetail> FlightDetail { get; set; }

        public FlightDBContext(DbContextOptions<FlightDBContext> options)
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
            modelBuilder.Entity<Destination>(entity =>
            {
                entity.HasKey(e => e.DestId);

                entity.Property(e => e.Destination1)
                    .IsRequired()
                    .HasColumnName("Destination")
                    .IsUnicode(false);

                entity.HasOne(d => d.Flight)
                    .WithMany(p => p.Destination)
                    .HasForeignKey(d => d.FlightId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Destination_Flight");
            });

            modelBuilder.Entity<FlBooking>(entity =>
            {
                entity.HasKey(e => e.BookingId);

                entity.Property(e => e.BookingId).ValueGeneratedNever();

                entity.Property(e => e.BookDate).HasColumnType("date");

                entity.Property(e => e.FlightType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PayType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ReturnDate).HasColumnType("date");

                entity.HasOne(d => d.Detail)
                    .WithMany(p => p.FlBooking)
                    .HasForeignKey(d => d.DetailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FlBooking_Flight_Detail");
            });

            modelBuilder.Entity<FlCompany>(entity =>
            {
                entity.HasKey(e => e.Cid);

                entity.Property(e => e.Cid).HasColumnName("CId");

                entity.Property(e => e.CompanyName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Picture).IsRequired();
            });

            modelBuilder.Entity<Flight>(entity =>
            {
                entity.Property(e => e.Locale)
                    .IsRequired()
                    .IsUnicode(false);
            });

            modelBuilder.Entity<FlightDetail>(entity =>
            {
                entity.HasKey(e => e.DetailId);

                entity.ToTable("Flight_Detail");

                entity.Property(e => e.Cid).HasColumnName("CId");

                entity.Property(e => e.Departure)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.Path)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.Price)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.ReturnTrip)
                    .IsRequired()
                    .IsUnicode(false);

                entity.HasOne(d => d.C)
                    .WithMany(p => p.FlightDetail)
                    .HasForeignKey(d => d.Cid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Flight_Detail_FlCompany");

                entity.HasOne(d => d.Dest)
                    .WithMany(p => p.FlightDetail)
                    .HasForeignKey(d => d.DestId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Flight_Detail_Destination");
            });
        }
    }
}
