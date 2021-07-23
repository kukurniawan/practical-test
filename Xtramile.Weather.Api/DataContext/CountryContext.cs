using Microsoft.EntityFrameworkCore;

namespace Xtramile.Weather.Api.DataContext
{
    public class CountryContext : DbContext
    {
        public CountryContext(DbContextOptions<CountryContext> options)
            : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>(x =>
                x.HasOne(c => c.Country)
                    .WithMany(c => c.Cities)
                    .HasForeignKey(b => b.CountryCode));
        }

        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
    }
}