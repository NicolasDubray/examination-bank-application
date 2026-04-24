using Bank.Domain.Entities;

namespace Bank.Domain.Interfaces;

public interface IAccountRepository
{
    Account? GetById(int id);
    Account? GetByAccountNumber(string accountNumber);
    List<Account> GetByCustomerId(int customerId);
    List<Account> GetAll();
    void Add(Account account);
    void Update(Account account);
    int Count();
}
