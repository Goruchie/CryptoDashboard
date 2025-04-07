using CryptoDashboard.Context;
using CryptoDashboard.Models;

namespace CryptoDashboard.Services
{
    public class CryptoJobService
    {
        private readonly AppDbContext _context;

        public CryptoJobService(AppDbContext context)
        {
            _context = context;
        }

        public void FetchAndStorePrices()
        {
            using (var httpClient = new HttpClient())
            {
                try
                {

                    var response = httpClient
                        .GetAsync("https://api.coingecko.com/api/v3/simple/price?ids=bitcoin,ethereum&vs_currencies=usd")
                        .Result;

                    if (response.IsSuccessStatusCode)
                    {

                        var content = response.Content.ReadAsStringAsync().Result;
                        var prices = System.Text.Json.JsonSerializer
    .Deserialize<Dictionary<string, Dictionary<string, decimal>>>(content);

                        if (prices != null)
                        {
                            foreach (var price in prices)
                            {
                                var cryptoPrice = new CryptoPrice
                                {
                                    CryptoCurrencyId = price.Key == "bitcoin" ? 1 : 2, 
                                    Date = DateTime.UtcNow,
                                    Price = price.Value["usd"],
                                    Volume = 0 
                                };

                                _context.CryptoPrices.Add(cryptoPrice); 
                            }

                            _context.SaveChanges(); 
                            Console.WriteLine("Prices fetched and stored successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Failed to deserialize the response or no data was returned.");
                        }

                    }
                    else
                    {
                        Console.WriteLine($"Error while calling CoinGecko API: {response.StatusCode}");
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
