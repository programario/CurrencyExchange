using CurrencyExchange.Models;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchange.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly CurrencyExchangeDbContext _dbContext;

        public UserController(CurrencyExchangeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("create")]
        public async Task<ActionResult<string>> CreateUserAsync([FromBody] UserModel user)
        {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return $"User {user.Username} created successfully";
        }

        [HttpGet("{username}/accounts")]
        public ActionResult<List<AccountModel>> GetUserAccounts(string username)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return NotFound($"User {username} not found");

            return user.Accounts;
        }

        [HttpPost("{username}/accounts/add")]
        public async Task<ActionResult<string>> AddAccount(string username, [FromBody] AccountModel account)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return NotFound($"User {username} not found");

            // Проверка наличия счета для данной валюты у пользователя
            if (user.Accounts.Any(a => a.Currency == account.Currency))
                return BadRequest($"Account with currency {account.Currency} already exists for user {username}");

            user.Accounts.Add(account);
            await _dbContext.SaveChangesAsync();
            return $"Account with currency {account.Currency} added for user {username}";

        }

        [HttpPost("{username}/accounts/update-balance")]
        public async Task<ActionResult<string>> UpdateAccountBalance(string username, [FromBody] AccountBalanceUpdateModel updateModel)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return NotFound($"User {username} not found");

            var account = user.Accounts.FirstOrDefault(a => a.Currency == updateModel.Currency);
            if (account == null)
                return NotFound($"Account with currency {updateModel.Currency} not found for user {username}");

            // Обновление баланса
            account.Balance += updateModel.Amount;
            await _dbContext.SaveChangesAsync();
            return $"Balance updated successfully for user {username}, currency {updateModel.Currency}";
        }


    }
}


