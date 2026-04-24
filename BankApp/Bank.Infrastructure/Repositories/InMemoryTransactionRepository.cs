using Bank.Domain.Entities;
using Bank.Domain.Enums;
using Bank.Domain.Interfaces;

namespace Bank.Infrastructure.Repositories;

public class InMemoryTransactionRepository : ITransactionRepository
{
    private readonly List<Transaction> _transactions = new();
    private int _nextId = 1;

    public Transaction? GetById(int id) => _transactions.FirstOrDefault(t => t.Id == id);
    public List<Transaction> GetByAccountId(int accountId) => _transactions.Where(t => t.AccountId == accountId).ToList();
    public List<Transaction> GetAll() => _transactions.ToList();

    // BUG_TARGET: Add
    public void Add(Transaction transaction)
    {
        transaction.Id = _nextId++;
        _transactions.Add(transaction);
    }

    public int Count() => _transactions.Count;
}
