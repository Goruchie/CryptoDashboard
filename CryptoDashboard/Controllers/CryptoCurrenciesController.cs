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


        [HttpGet("prices-volumes")]
        public async Task<IActionResult> GetCryptoPrices()
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync("https://api.coingecko.com/api/v3/simple/price?ids=bitcoin,ethereum&vs_currencies=usd&include_market_cap=false&include_24hr_vol=true");
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
                httpClient.DefaultRequestHeaders.Add("User-Agent", "CryptoDashboardApp/1.0");

                var response = await httpClient.GetAsync("https://api.coingecko.com/api/v3/coins/markets?vs_currency=usd&ids=bitcoin,ethereum&order=market_cap_desc&per_page=100&page=1&sparkline=false");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    var document = System.Text.Json.JsonDocument.Parse(content);
                    var jsonArray = document.RootElement.EnumerateArray();

                    foreach (var price in jsonArray)
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

                    await _context.SaveChangesAsync();
                    return Ok("Prices and volume fetched and stored successfully!");
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
            startDate = ConvertToUtc(startDate.Date); 
            endDate = ConvertToUtc(endDate.Date.AddDays(1).AddTicks(-1)); 

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

        private DateTime ConvertToUtc(DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Unspecified)
            {
                return TimeZoneInfo.ConvertTimeToUtc(dateTime, TimeZoneInfo.Local);
            }
            return dateTime.ToUniversalTime();
        }

        [HttpGet("max-price/{id}")]
        public async Task<IActionResult> GetMaxPrice(int id, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            startDate = ConvertToUtc(startDate.Date); 
            endDate = ConvertToUtc(endDate.Date.AddDays(1).AddTicks(-1)); 

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
            startDate = ConvertToUtc(startDate.Date); 
            endDate = ConvertToUtc(endDate.Date.AddDays(1).AddTicks(-1)); 

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
