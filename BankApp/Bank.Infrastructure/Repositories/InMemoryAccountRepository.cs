using Bank.Domain.Entities;
using Bank.Domain.Interfaces;

namespace Bank.Infrastructure.Repositories;

public class InMemoryAccountRepository : IAccountRepository
{
    private readonly List<Account> _accounts = new();

    public Account? GetById(int id) => _accounts.FirstOrDefault(a => a.Id == id);
    // BUG_TARGET: GetByAccountNumber
    public Account? GetByAccountNumber(string accountNumber) => _accounts.FirstOrDefault(a => string.Equals(a.AccountNumber, accountNumber, StringComparison.OrdinalIgnoreCase));
    // BUG_TARGET: GetByCustomerId
    public List<Account> GetByCustomerId(int customerId) => _accounts.Where(a => a.CustomerId == customerId).ToList();
    public List<Account> GetAll() => _accounts.ToList();
    public void Add(Account account) => _accounts.Add(account);

    // BUG_TARGET: Update
    public void Update(Account account)
    {
        var index = _accounts.FindIndex(a => a.Id == account.Id);
        if (index >= 0)
            _accounts[index] = account;
    }
}
