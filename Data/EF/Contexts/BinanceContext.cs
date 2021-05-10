using Data.Configuration;
using Data.Data;
using Microsoft.EntityFrameworkCore;

namespace Data.EF.Contexts
{
    public class BinanceContext : DbContext
    {
        public BinanceContext(DbContextOptions<BinanceContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AssetEntityConfiguration());
        }

        public DbSet<Asset> Assets { get; set; }
    }
}
