namespace CurrencyExchange.Models
{
    public class CurrencyConversionModel
    {
        public string Username { get; set; }
        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }
        public double ExchangeRate { get; set; }
        public double Amount { get; set; }
        public double CommissionPercentage { get; set; }
    }
}
