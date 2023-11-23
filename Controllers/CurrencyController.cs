using Microsoft.AspNetCore.Mvc;
using CurrencyExchange.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchange.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly CurrencyExchangeDbContext _dbContext;

        public CurrencyController(CurrencyExchangeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("list")]
        public async Task<ActionResult<List<CurrencyModel>>> GetCurrencies()
        {
            return await _dbContext.GetCurrenciesAsync();
        }

        [HttpPost("add")]
        public async Task<ActionResult<string>> AddCurrency([FromBody] CurrencyModel currency)
        {
            // Проверка наличия валюты с таким кодом
            if (_dbContext.Currencies.Any(c => c.Code == currency.Code))
                return BadRequest($"Currency with code {currency.Code} already exists");

            _dbContext.Currencies.Add(currency);
            await _dbContext.SaveChangesAsync();
            return $"Currency {currency.Name} ({currency.Code}) added successfully";
        }

        [HttpPost("convert")]
        public ActionResult<double> ConvertCurrency([FromBody] CurrencyConversionModel conversionModel)
        {
            // Валидация входных данных
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(conversionModel, new ValidationContext(conversionModel), validationResults, true))
            {
                var errorMessages = string.Join(", ", validationResults.Select(vr => vr.ErrorMessage));
                return BadRequest($"Invalid input: {errorMessages}");
            }

            var fromCurrency = _dbContext.Currencies.FirstOrDefault(c => c.Code == conversionModel.FromCurrency);
            var toCurrency = _dbContext.Currencies.FirstOrDefault(c => c.Code == conversionModel.ToCurrency);

            if (fromCurrency == null || toCurrency == null)
                return BadRequest("Invalid currencies specified");

            // Проверка наличия валюты с заданным кодом в аккаунте пользователя
            var user = _dbContext.Users.FirstOrDefault(u => u.Username == conversionModel.Username);
            if (user == null || !user.Accounts.Any(a => a.Currency == conversionModel.FromCurrency))
                return BadRequest($"User {conversionModel.Username} does not have an account with currency {conversionModel.FromCurrency}");

            // Получение суммы на счету пользователя
            var account = user.Accounts.First(a => a.Currency == conversionModel.FromCurrency);
            var amount = account.Balance;

            // Расчет конверсии
            var convertedAmount = amount * conversionModel.ExchangeRate;

            // Удостоверьтесь, что у вас достаточно средств для конвертации
            if (amount < conversionModel.Amount)
                return BadRequest($"Insufficient funds in {conversionModel.FromCurrency} account for user {conversionModel.Username}");

            // Рассчет комиссии как процента от полученной суммы во второй валюте
            var commissionPercentage = conversionModel.CommissionPercentage > 0 ? conversionModel.CommissionPercentage : 0.05; // Значение по умолчанию: 0.05%
            var commission = convertedAmount * (commissionPercentage / 100.0);

            // Списание суммы в первой валюте с учетом комиссии
            account.Balance -= (conversionModel.Amount + commission);

            // Зачисление сконвертированной суммы во вторую валюту
            var toCurrencyAccount = user.Accounts.First(a => a.Currency == conversionModel.ToCurrency);
            toCurrencyAccount.Balance += (convertedAmount - commission);

            return convertedAmount;
        }
    }
}


