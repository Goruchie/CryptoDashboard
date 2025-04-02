using System.ComponentModel.DataAnnotations;

namespace CryptoDashboard.Models
{
    public class CryptoPrice
    {
        public int Id { get; set; }
        public int CryptoCurrencyId { get; set; }
        [Required] 
        public required DateTime Date { get; set; }
        [Required] 
        public required decimal Price { get; set; }
        [Required] 
        public required decimal Volume { get; set; }
    }
}
