using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BookingServer.Models.Users
{
    public partial class UserDBContext : DbContext
    {
        public virtual DbSet<User> User { get; set; }

        public UserDBContext(DbContextOptions<UserDBContext> options) : base(options)
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
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Email)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(110)
                    .IsUnicode(false);

                entity.Property(e => e.Surname)
                    .IsRequired()
                    .IsUnicode(false);
            });
        }
    }
}
