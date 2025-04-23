using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CryptoDashboard.Models
{
    [Table("cryptocurrencies")]
    public class CryptoCurrency
    {
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("name")]
        public required string Name { get; set; }
        [Required]
        [Column("symbol")]
        public required string Symbol { get; set; }
    }
}
