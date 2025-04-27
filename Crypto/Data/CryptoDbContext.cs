
using Crypto.Models;
using Microsoft.EntityFrameworkCore;

namespace Crypto.Data;

public class CryptoDbContext : DbContext
{
    public CryptoDbContext(DbContextOptions<CryptoDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<Cryptocurrency> Cryptocurrencies { get; set; }
    public DbSet<PortfolioItem> PortfolioItems { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<PriceHistory> PriceHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        modelBuilder.Entity<User>()
            .HasOne(u => u.Wallet)
            .WithOne(w => w.User)
            .HasForeignKey<Wallet>(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        
        modelBuilder.Entity<PortfolioItem>()
            .HasOne(pi => pi.Wallet)
            .WithMany(w => w.Portfolio)
            .HasForeignKey(pi => pi.WalletId)
            .OnDelete(DeleteBehavior.NoAction);

        
        modelBuilder.Entity<PortfolioItem>()
            .HasOne(pi => pi.Cryptocurrency)
            .WithMany()
            .HasForeignKey(pi => pi.CryptocurrencyId)
            .OnDelete(DeleteBehavior.NoAction);

        
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Cryptocurrency)
            .WithMany()
            .HasForeignKey(t => t.CryptocurrencyId)
            .OnDelete(DeleteBehavior.NoAction);

       
        modelBuilder.Entity<PriceHistory>()
            .HasOne(ph => ph.Cryptocurrency)
            .WithMany(c => c.PriceHistory)
            .HasForeignKey(ph => ph.CryptocurrencyId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}