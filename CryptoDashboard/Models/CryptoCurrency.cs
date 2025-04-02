using System.ComponentModel.DataAnnotations;

namespace CryptoDashboard.Models
{
    public class CryptoCurrency
    {
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }
        [Required] 
        public required string Symbol { get; set; }
    }
}
