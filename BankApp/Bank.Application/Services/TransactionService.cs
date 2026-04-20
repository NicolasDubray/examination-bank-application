using Bank.Domain.Entities;
using Bank.Domain.Enums;
using Bank.Domain.Interfaces;

namespace Bank.Application.Services;

public class TransactionService
{
    private readonly ITransactionRepository _transactionRepository;

    public TransactionService(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public List<Transaction> GetTransactionsByAccountId(int accountId)
    {
        return _transactionRepository.GetByAccountId(accountId);
    }

    // MISSING_TARGET: FilterTransactionsByDate
    public List<Transaction> FilterTransactionsByDate(int accountId, DateTime fromDate, DateTime toDate)
    {
        if (fromDate > toDate)
            throw new ArgumentException("From date must be before to date.");

        var transactions = _transactionRepository.GetByAccountId(accountId);
        return transactions
            .Where(t => t.Date >= fromDate && t.Date <= toDate)
            .OrderBy(t => t.Date)
            .ToList();
    }

    // BUG_TARGET: GetTotalDeposits
    public decimal GetTotalDeposits(int accountId)
    {
        var transactions = _transactionRepository.GetByAccountId(accountId);
        return transactions
            .Where(t => t.TransactionType == TransactionType.Deposit)
            .Sum(t => t.Amount);
    }

    // BUG_TARGET: GetTotalWithdrawals
    public decimal GetTotalWithdrawals(int accountId)
    {
        var transactions = _transactionRepository.GetByAccountId(accountId);
        return transactions
            .Where(t => t.TransactionType == TransactionType.Withdrawal)
            .Sum(t => t.Amount);
    }

    // MISSING_TARGET: GetTransactionCount
    public int GetTransactionCount(int accountId)
    {
        return _transactionRepository.GetByAccountId(accountId).Count;
    }

    // BUG_TARGET: GetLargestTransaction
    public Transaction? GetLargestTransaction(int accountId)
    {
        var transactions = _transactionRepository.GetByAccountId(accountId);
        if (transactions.Count == 0)
            return null;

        return transactions.OrderByDescending(t => t.Amount).First();
    }

    // MISSING_TARGET: GetHighValueTransactions
    public List<Transaction> GetHighValueTransactions(int accountId, decimal threshold, DateTime from, DateTime to)
    {
        if (from > to)
            throw new ArgumentException("From date must be before to date.");
        if (threshold < 0)
            throw new ArgumentException("Threshold cannot be negative.");

        return _transactionRepository.GetByAccountId(accountId)
            .Where(t => t.Amount >= threshold)
            .OrderByDescending(t => t.Amount)
            .ToList();
    }
}
