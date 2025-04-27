using Crypto.Data;
using Crypto.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Crypto.Services;

public class PriceUpdateService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Random _random = new Random();

    public PriceUpdateService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<CryptoDbContext>();

                var cryptocurrencies = await context.Cryptocurrencies.ToListAsync();
                foreach (var crypto in cryptocurrencies)
                {
                    
                    double changePercent = (_random.NextDouble() * 0.2 - 0.1); 
                    double newPrice = crypto.CurrentPrice * (1 + changePercent);
                    crypto.CurrentPrice = Math.Round(newPrice, 2);

                   
                    var priceHistory = new PriceHistory
                    {
                        CryptocurrencyId = crypto.Id,
                        Price = crypto.CurrentPrice,
                        Timestamp = DateTime.UtcNow
                    };
                    context.PriceHistories.Add(priceHistory);
                }

                await context.SaveChangesAsync();
            }

            await Task.Delay(30000, stoppingToken);
        }
    }
}