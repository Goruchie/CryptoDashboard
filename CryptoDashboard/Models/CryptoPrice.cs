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

        [Column("date", TypeName = "timestamp with time zone")]
        public  DateTime Date { get; set; }

        [Column("price")]
        public  decimal Price { get; set; }

        [Column("volume")]
        public decimal Volume { get; set; }
    }
}
