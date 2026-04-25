using Bank.Domain.Entities;

namespace Bank.Domain.Interfaces;

public interface ITransactionRepository
{
    Transaction? GetById(int id);
    List<Transaction> GetByAccountId(int accountId);
    List<Transaction> GetAll();
    void Add(Transaction transaction);
    int Count();
}
