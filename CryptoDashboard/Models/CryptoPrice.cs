namespace CryptoDashboard.Models
{
    public class CryptoPrice
    {
        public int Id { get; set; }
        public int CryptoCurrencyId { get; set; }
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public decimal Volume { get; set; }
    }
}
