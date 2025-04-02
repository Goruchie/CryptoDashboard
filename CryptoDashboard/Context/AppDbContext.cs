using CryptoDashboard.Models;
using Microsoft.EntityFrameworkCore;

namespace CryptoDashboard.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        public DbSet<CryptoCurrency> CryptoCurrencies { get; set; }
        public DbSet<CryptoPrice> CryptoPrices { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CryptoPrice>()
                .HasOne<CryptoCurrency>()
                .WithMany()
                .HasForeignKey(p => p.CryptoCurrencyId);
        }

    }
}
