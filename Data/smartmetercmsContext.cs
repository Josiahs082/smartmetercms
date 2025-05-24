using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public DbSet<smartmetercms.Models.User> User { get; set; } = default!;
        public DbSet<smartmetercms.Models.EnergyUsage> EnergyUsage { get; set; } = default!;
        public DbSet<smartmetercms.Models.Bill> Bill { get; set; } = default!;
        public DbSet<smartmetercms.Models.Payments> Payments { get; set; } = default!;
        public DbSet<smartmetercms.Models.IntervalEnergyUsage> IntervalEnergyUsage { get; set; } = default!;
        public DbSet<smartmetercms.Models.PowerQuality> PowerQuality { get; set; }

    }
}
