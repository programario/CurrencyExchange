using CurrencyExchange.Models;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchange
{
    public class CurrencyExchangeDbContext : DbContext
    {
        public CurrencyExchangeDbContext(DbContextOptions<CurrencyExchangeDbContext> options) : base(options)
        {
        }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<CurrencyModel> Currencies { get; set; }
        public DbSet<AccountModel> Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }

        public async Task<List<UserModel>> GetUsersAsync()
        {
            return await Users.ToListAsync();
        }
        public async Task<List<CurrencyModel>> GetCurrenciesAsync()
        {
            return await Currencies.ToListAsync();
        }
        public async Task<List<AccountModel>> GetAccountsAsync()
        {
            return await Accounts.ToListAsync();
        }
    }
}
