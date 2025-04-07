using CryptoDashboard.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CryptoDashboard.Context;

namespace CryptoDashboard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CryptoCurrenciesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CryptoCurrenciesController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet("prices")]
        public async Task<IActionResult> GetCryptoPrices()
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync("https://api.coingecko.com/api/v3/simple/price?ids=bitcoin,ethereum&vs_currencies=usd");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return Ok(content);
                }
                return StatusCode((int)response.StatusCode, "Error fetching data from CoinGecko.");
            }
        }
        [HttpPost("add-cryptocurrency")]
        public async Task<IActionResult> AddCryptocurrency([FromBody] CryptoCurrency cryptocurrency)
        {
            _context.CryptoCurrencies.Add(cryptocurrency);
            await _context.SaveChangesAsync();
            return Ok("Cryptocurrency added successfully!");
        }
        [HttpGet("prices/{id}")]
        public async Task<IActionResult> GetCryptoPricesById(int id)
        {
            var prices = await _context.CryptoPrices
                .Where(p => p.CryptoCurrencyId == id)
                .OrderBy(p => p.Date)
                .ToListAsync();

            return Ok(prices);
        }
        [HttpPost("fetch-and-store-prices")]
        public async Task<IActionResult> FetchAndStorePrices()
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync("https://api.coingecko.com/api/v3/simple/price?ids=bitcoin,ethereum&vs_currencies=usd");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    var prices = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, decimal>>>(content);

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

                        await _context.SaveChangesAsync();
                        return Ok("Prices fetched and stored successfully!");
                    }
                    else
                    {
                        return BadRequest("Failed to deserialize the response or no data was returned.");
                    }
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Failed to fetch data from CoinGecko.");
                }
            }
        }
        [HttpGet("average-price/{id}")]
        public async Task<IActionResult> GetAveragePrice(int id, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var prices = await _context.CryptoPrices
                .Where(p => p.CryptoCurrencyId == id && p.Date >= startDate && p.Date <= endDate)
                .ToListAsync();

            if (!prices.Any())
            {
                return NotFound("No prices found for the specified criteria.");
            }

            var averagePrice = prices.Average(p => p.Price);

            return Ok(new { AveragePrice = averagePrice });
        }
        [HttpGet("max-price/{id}")]
        public async Task<IActionResult> GetMaxPrice(int id, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var prices = await _context.CryptoPrices
                .Where(p => p.CryptoCurrencyId == id && p.Date >= startDate && p.Date <= endDate)
                .ToListAsync();

            if (!prices.Any())
            {
                return NotFound("No prices found for the specified criteria.");
            }

            var maxPrice = prices.Max(p => p.Price);

            return Ok(new { MaxPrice = maxPrice });
        }
        [HttpGet("historical-prices/{id}")]
        public async Task<IActionResult> GetHistoricalPrices(int id, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var prices = await _context.CryptoPrices
                .Where(p => p.CryptoCurrencyId == id && p.Date >= startDate && p.Date <= endDate)
                .OrderBy(p => p.Date)
                .ToListAsync();

            if (!prices.Any())
            {
                return NotFound("No historical prices found for the specified criteria.");
            }

            return Ok(prices);
        }
    }
}
