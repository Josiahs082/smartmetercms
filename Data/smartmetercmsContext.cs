using Microsoft.EntityFrameworkCore;
using smartmetercms.Models;

namespace smartmetercms.Data
{
    public class smartmetercmsContext : DbContext
    {
        public smartmetercmsContext(DbContextOptions<smartmetercmsContext> options)
            : base(options)
        {
        }

        public DbSet<User> User { get; set; }
        public DbSet<IntervalEnergyUsage> IntervalEnergyUsage { get; set; }
        public DbSet<EnergyUsage> EnergyUsage { get; set; }
        public DbSet<Bill> Bill { get; set; }
        public DbSet<Payments> Payments { get; set; }
        public DbSet<PowerQuality> PowerQuality { get; set; }
        public DbSet<MeterStatus> MeterStatus { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define the relationship between User and Bill
            modelBuilder.Entity<User>()
                .HasMany(u => u.Bills)
                .WithOne() // No navigation property in Bill pointing back to User
                .HasForeignKey(b => b.MeterID) // Bill.MeterID links to User.MeterID
                .OnDelete(DeleteBehavior.Cascade);

            // Ensure MeterStatus uses MeterID as the primary key
            modelBuilder.Entity<MeterStatus>()
                .HasKey(ms => ms.MeterID);
        }
    }
}