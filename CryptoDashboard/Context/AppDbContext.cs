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
        public override int SaveChanges()
        {
            ConvertDateTimesToUtc();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ConvertDateTimesToUtc();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void ConvertDateTimesToUtc()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is CryptoPrice &&
                            (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var cryptoPrice = (CryptoPrice)entry.Entity;

                if (cryptoPrice.Date.Kind == DateTimeKind.Unspecified)
                {
                    cryptoPrice.Date = DateTime.SpecifyKind(cryptoPrice.Date, DateTimeKind.Utc);
                }
                else
                {
                    cryptoPrice.Date = cryptoPrice.Date.ToUniversalTime();
                }
            }
        }

    }
}
