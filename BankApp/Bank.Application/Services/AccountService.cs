using System.Security.Principal;

using Bank.Domain.Entities;
using Bank.Domain.Enums;
using Bank.Domain.Interfaces;

namespace Bank.Application.Services;

public class AccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;
    private int _nextAccountId = 1;

    public AccountService(IAccountRepository accountRepository, ITransactionRepository transactionRepository)
    {
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
    }

    // BUG_TARGET: CreateAccount
    public Account CreateAccount(int customerId, AccountType accountType, string accountNumber)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
            throw new ArgumentException("Account number cannot be empty.");

        var existing = _accountRepository.GetByAccountNumber(accountNumber);
        if (existing != null)
            throw new InvalidOperationException("Account number already exists.");

        var account = new Account
        {
            Id = _nextAccountId++,
            CustomerId = customerId,
            AccountType = accountType,
            AccountNumber = accountNumber,
            Balance = 0,
            CreatedDate = DateTime.Now
        };

        _accountRepository.Add(account);
        return account;
    }

    public Account? GetAccountById(int accountId)
    {
        return _accountRepository.GetById(accountId);
    }

    public Account? GetAccountByNumber(string accountNumber)
    {
        return _accountRepository.GetByAccountNumber(accountNumber);
    }

    public List<Account> GetAccountsByCustomer(int customerId)
    {
        return _accountRepository.GetByCustomerId(customerId);
    }

    // BUG_TARGET: Deposit
    public void Deposit(int accountId, decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Deposit amount must be positive.");

        var account = _accountRepository.GetById(accountId);
        if (account == null)
            throw new InvalidOperationException("Account not found.");

        account.Balance += amount;
        _accountRepository.Update(account);

        var transaction = new Transaction
        {
            Id = 0,
            AccountId = accountId,
            Amount = amount,
            TransactionType = TransactionType.Deposit,
            Date = DateTime.Now,
            Description = $"Deposit of {amount:C}"
        };
        _transactionRepository.Add(transaction);
    }

    // BUG_TARGET: Withdraw
    public void Withdraw(int accountId, decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Withdrawal amount must be positive.");

        var account = _accountRepository.GetById(accountId);
        if (account == null)
            throw new InvalidOperationException("Account not found.");

        if (account.Balance < amount)
            throw new InvalidOperationException("Insufficient funds.");

        account.Balance -= amount;
        _accountRepository.Update(account);

        var transaction = new Transaction
        {
            Id = 0,
            AccountId = accountId,
            Amount = amount,
            TransactionType = TransactionType.Withdrawal,
            Date = DateTime.Now,
            Description = $"Withdrawal of {amount:C}"
        };
        _transactionRepository.Add(transaction);
    }

    // MISSING_TARGET: Transfer
    public void Transfer(int fromAccountId, int toAccountId, decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Transfer amount must be positive.");

        var fromAccount = _accountRepository.GetById(fromAccountId);
        if(fromAccount == null)
            throw new InvalidOperationException("Source account not found.");

        var toAccount = _accountRepository.GetById(toAccountId);
        if (toAccount == null)
            throw new InvalidOperationException("Destination account not found.");

        if (fromAccountId == toAccountId)
            throw new ArgumentException("Cannot transfer from and to the same account.");

        if (fromAccount.Balance < amount)
            throw new InvalidOperationException("Insufficient funds.");

        fromAccount.Balance -= amount;
        _accountRepository.Update(fromAccount);

        toAccount.Balance += amount;
        _accountRepository.Update(toAccount);

        var transaction = new Transaction
        {
            Id = 0,
            AccountId = fromAccount.Id,
            Amount = -amount,
            TransactionType = TransactionType.Transfer,
            Date = DateTime.Now,
            Description = $"Transfer of {amount:C}"
        };
        _transactionRepository.Add(transaction);

        transaction = new Transaction
        {
            Id = 0,
            AccountId = toAccount.Id,
            Amount = amount,
            TransactionType = TransactionType.Transfer,
            Date = DateTime.Now,
            Description = $"Transfer of {amount:C}"
        };
        _transactionRepository.Add(transaction);
    }

    // BUG_TARGET: GetBalance
    public decimal GetBalance(int accountId)
    {
        var account = _accountRepository.GetById(accountId);
        if (account == null)
            throw new InvalidOperationException("Account not found.");

        return account.Balance;
    }

    // MISSING_TARGET: CalculateInterest
    public decimal CalculateInterest(int accountId, decimal annualRate, int months)
    {
        if (annualRate < 0)
            throw new ArgumentException("Interest rate cannot be negative.");
        if (months <= 0)
            throw new ArgumentException("Months must be positive.");

        var account = _accountRepository.GetById(accountId);
        if (account == null)
            throw new InvalidOperationException("Account not found.");

        var monthlyRate = annualRate / 12m / 100m;
        return Math.Round(account.Balance * monthlyRate * months, 2);
    }

    // MISSING_TARGET: GetAccountStatement
    public List<Transaction> GetAccountStatement(int accountId, DateTime fromDate, DateTime toDate)
    {
        var account = _accountRepository.GetById(accountId);
        if (account == null)
            throw new InvalidOperationException("Account not found.");

        if (fromDate > toDate)
            throw new ArgumentException("From date must be before to date.");

        var transactions = _transactionRepository.GetByAccountId(accountId);
        return transactions
            .Where(t => t.Date >= fromDate && t.Date <= toDate)
            .OrderBy(t => t.Date)
            .ToList();
    }

    // MISSING_TARGET: GetTotalBalance
    public decimal GetTotalBalance(int customerId)
    {
        var accounts = _accountRepository.GetByCustomerId(customerId);
        return accounts.Sum(a => a.Balance);
    }

    // MISSING_TARGET: GetMonthlyReport
    public string GetMonthlyReport(int accountId, int month, int year)
    {
        if (month < 1 || month > 12)
            throw new ArgumentException("Month must be between 1 and 12.");

        var account = _accountRepository.GetById(accountId);
        if (account == null)
            throw new InvalidOperationException("Account not found.");

        var from = new DateTime(year, month, 1);
        var to = from.AddMonths(1).AddTicks(-1);

        var transactions = _transactionRepository.GetByAccountId(accountId)
            .Where(t => t.Date >= from && t.Date <= to)
            .ToList();

        var deposits = transactions
            .Where(t => t.TransactionType == TransactionType.Deposit)
            .Sum(t => t.Amount);
        var withdrawals = transactions
            .Where(t => t.TransactionType == TransactionType.Withdrawal)
            .Sum(t => t.Amount);
        var net = deposits - withdrawals;

        return $"{account.AccountNumber} {year}-{month:D2}: In: {deposits:F2}, Ut: {withdrawals:F2}, Netto: {net:F2}";
    }

    // MISSING_TARGET: TransferWithFee
    public void TransferWithFee(int fromAccountId, int toAccountId, decimal amount, decimal feePercent)
    {
        var fromAccount = _accountRepository.GetById(fromAccountId);
        if (fromAccount == null)
            throw new InvalidOperationException("Source account not found.");

        var transactionFee = amount * (feePercent / 100);
        if (fromAccount.Balance < amount + transactionFee)
            throw new InvalidOperationException("Insufficient funds.");

        Transfer(fromAccountId, toAccountId, amount);

        fromAccount.Balance -= transactionFee;
        _accountRepository.Update(fromAccount);

        var transaction = new Transaction
        {
            Id = 0,
            AccountId = fromAccount.Id,
            Amount = transactionFee,
            TransactionType = TransactionType.Withdrawal,
            Date = DateTime.Now,
            Description = $"Charge of {transactionFee:C} as transaction fee for amount {amount:C}"
        };
        _transactionRepository.Add(transaction);
    }
}
