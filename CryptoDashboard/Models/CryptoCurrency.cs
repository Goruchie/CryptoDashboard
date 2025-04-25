using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CryptoDashboard.Models
{
    [Table("cryptocurrencies")]
    public class CryptoCurrency
    {
        [Column("id")]
        public int Id { get; set; }

        
        [Column("name")]
        public  string Name { get; set; }
      
        [Column("symbol")]
        public  string Symbol { get; set; }
    }
}
