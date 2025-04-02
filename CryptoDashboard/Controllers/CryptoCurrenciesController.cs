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


    }
}
