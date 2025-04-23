using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CryptoDashboard.Models
{
    [Table("cryptoprices")]

    public class CryptoPrice
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("cryptocurrencyid")]
        public int CryptoCurrencyId { get; set; }
        [Required]
        [Column("date", TypeName = "timestamp with time zone")]
        public required DateTime Date { get; set; }
        [Required]
        [Column("price")]
        public required decimal Price { get; set; }
        [Required]
        [Column("volume")]
        public required decimal Volume { get; set; }
    }
}
