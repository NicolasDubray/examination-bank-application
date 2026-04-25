// Examination: Nicolas Dubray
// Generated: 2026-04-02
// Domain: Bank

using Bank.Application.Services;
using Bank.Domain.Entities;
using Bank.Domain.Enums;
using Bank.Infrastructure.Factories;
using Bank.Infrastructure.Repositories;

namespace Bank.Tests;

public class TransactionServiceTests
{
    private (TransactionService service, AccountService accountService, InMemoryTransactionRepository transactionRepo) CreateServices()
    {
        var accountRepo = new InMemoryAccountRepository();
        var transactionRepo = new InMemoryTransactionRepository();
        var accountRuleFactory = new AccountRuleFactory();
        var accountRuleService = new AccountRuleService(accountRuleFactory, accountRepo);
        var accountService = new AccountService(accountRepo, transactionRepo, accountRuleService);
        var transactionService = new TransactionService(transactionRepo);
        return (transactionService, accountService, transactionRepo);
    }

    [Fact]
    public void GetTransactionsByAccountId_ReturnsCorrectTransactions()
    {
        var (service, accountService, _) = CreateServices();
        var account = accountService.CreateAccount(1, AccountType.Savings, "ACC-001");
        accountService.Deposit(account.Id, 500m);
        accountService.Withdraw(account.Id, 200m);

        var transactions = service.GetTransactionsByAccountId(account.Id);

        Assert.Equal(2, transactions.Count);
    }

    [Fact]
    public void GetTransactionsByAccountId_NoTransactions_ReturnsEmptyList()
    {
        var (service, accountService, _) = CreateServices();
        var account = accountService.CreateAccount(1, AccountType.Savings, "ACC-001");

        var transactions = service.GetTransactionsByAccountId(account.Id);

        Assert.Empty(transactions);
    }

    [Fact]
    public void FilterTransactionsByDate_ReturnsFilteredTransactions()
    {
        var (service, accountService, transactionRepo) = CreateServices();
        var account = accountService.CreateAccount(1, AccountType.Savings, "ACC-001");
        accountService.Deposit(account.Id, 500m);
        accountService.Deposit(account.Id, 300m);

        var allTransactions = transactionRepo.GetByAccountId(account.Id);
        var minDate = allTransactions.Min(t => t.Date).AddSeconds(-1);
        var maxDate = allTransactions.Max(t => t.Date).AddSeconds(1);

        var filtered = service.FilterTransactionsByDate(account.Id, minDate, maxDate);

        Assert.Equal(2, filtered.Count);
    }

    [Fact]
    public void FilterTransactionsByDate_InvalidRange_ThrowsArgumentException()
    {
        var (service, _, _) = CreateServices();

        Assert.Throws<ArgumentException>(() =>
            service.FilterTransactionsByDate(1, DateTime.Now, DateTime.Now.AddDays(-1)));
    }

    [Fact]
    public void GetTotalDeposits_ReturnsCorrectSum()
    {
        var (service, accountService, _) = CreateServices();
        var account = accountService.CreateAccount(1, AccountType.Savings, "ACC-001");
        accountService.Deposit(account.Id, 500m);
        accountService.Deposit(account.Id, 300m);
        accountService.Withdraw(account.Id, 100m);

        var total = service.GetTotalDeposits(account.Id);

        Assert.Equal(800m, total);
    }

    [Fact]
    public void GetTotalWithdrawals_ReturnsCorrectSum()
    {
        var (service, accountService, _) = CreateServices();
        var account = accountService.CreateAccount(1, AccountType.Savings, "ACC-001");
        accountService.Deposit(account.Id, 1000m);
        accountService.Withdraw(account.Id, 200m);
        accountService.Withdraw(account.Id, 150m);

        var total = service.GetTotalWithdrawals(account.Id);

        Assert.Equal(350m, total);
    }

    [Fact]
    public void GetTransactionCount_ReturnsCorrectCount()
    {
        var (service, accountService, _) = CreateServices();
        var account = accountService.CreateAccount(1, AccountType.Savings, "ACC-001");
        accountService.Deposit(account.Id, 500m);
        accountService.Deposit(account.Id, 300m);
        accountService.Withdraw(account.Id, 100m);

        var count = service.GetTransactionCount(account.Id);

        Assert.Equal(3, count);
    }

    [Fact]
    public void GetTransactionCount_NoTransactions_ReturnsZero()
    {
        var (service, _, _) = CreateServices();

        var count = service.GetTransactionCount(999);

        Assert.Equal(0, count);
    }

    [Fact]
    public void GetLargestTransaction_ReturnsLargest()
    {
        var (service, accountService, _) = CreateServices();
        var account = accountService.CreateAccount(1, AccountType.Savings, "ACC-001");
        accountService.Deposit(account.Id, 100m);
        accountService.Deposit(account.Id, 500m);
        accountService.Deposit(account.Id, 250m);

        var largest = service.GetLargestTransaction(account.Id);

        Assert.NotNull(largest);
        Assert.Equal(500m, largest.Amount);
    }

    [Fact]
    public void GetLargestTransaction_NoTransactions_ReturnsNull()
    {
        var (service, _, _) = CreateServices();

        var largest = service.GetLargestTransaction(999);

        Assert.Null(largest);
    }
}
