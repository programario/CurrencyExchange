namespace CurrencyExchange.Models
{
    public class UserModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public List<AccountModel> Accounts { get; set; } = new List<AccountModel>();
    }
}
