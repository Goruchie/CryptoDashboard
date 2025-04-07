using CryptoDashboard.Context;
using CryptoDashboard.Models;
using System.Text.Json;

namespace CryptoDashboard.Services
{
    public class CryptoJobService
    {
        private readonly AppDbContext _context;

        public CryptoJobService(AppDbContext context)
        {
            _context = context;
        }

        public async Task FetchAndStorePrices()
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    httpClient.DefaultRequestHeaders.Add("User-Agent", "CryptoDashboardApp/1.0");

                    var response = await httpClient.GetAsync("https://api.coingecko.com/api/v3/coins/markets?vs_currency=usd&ids=bitcoin,ethereum&order=market_cap_desc&per_page=100&page=1&sparkline=false");

                    Console.WriteLine($"Response status: {response.StatusCode}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();

                        Console.WriteLine($"Response content: {content}");

                        var document = JsonDocument.Parse(content);
                        var jsonArray = document.RootElement.EnumerateArray();

                        foreach (var price in jsonArray)
                        {
                            try
                            {
                                var cryptoPrice = new CryptoPrice
                                {
                                    CryptoCurrencyId = price.GetProperty("id").GetString() == "bitcoin" ? 1 : 2,
                                    Date = DateTime.UtcNow,
                                    Price = price.GetProperty("current_price").GetDecimal(),
                                    Volume = price.GetProperty("total_volume").GetDecimal()
                                };

                                _context.CryptoPrices.Add(cryptoPrice);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error processing crypto data: {ex.Message}");
                            }
                        }

                        await _context.SaveChangesAsync();
                        Console.WriteLine("Prices and volume fetched and stored successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to fetch data from CoinGecko. StatusCode: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in FetchAndStorePrices: {ex.Message}");
                }
            }
        }
    }
}