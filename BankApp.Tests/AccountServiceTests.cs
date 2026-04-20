// Examination: Nicolas Dubray
// Generated: 2026-04-02
// Domain: Bank

using Bank.Application.Services;
using Bank.Domain.Enums;
using Bank.Infrastructure.Repositories;

namespace Bank.Tests;

public class AccountServiceTests
{
    private AccountService CreateService(out InMemoryAccountRepository accountRepo, out InMemoryTransactionRepository transactionRepo)
    {
        accountRepo = new InMemoryAccountRepository();
        transactionRepo = new InMemoryTransactionRepository();
        return new AccountService(accountRepo, transactionRepo);
    }

    [Fact]
    public void CreateAccount_ValidInput_ReturnsAccount()
    {
        var service = CreateService(out _, out _);

        var account = service.CreateAccount(1, AccountType.Savings, "ACC-001");

        Assert.NotNull(account);
        Assert.Equal("ACC-001", account.AccountNumber);
        Assert.Equal(1, account.CustomerId);
        Assert.Equal(AccountType.Savings, account.AccountType);
        Assert.Equal(0, account.Balance);
    }

    [Fact]
    public void CreateAccount_EmptyAccountNumber_ThrowsArgumentException()
    {
        var service = CreateService(out _, out _);

        Assert.Throws<ArgumentException>(() => service.CreateAccount(1, AccountType.Savings, ""));
    }

    [Fact]
    public void CreateAccount_DuplicateAccountNumber_ThrowsInvalidOperationException()
    {
        var service = CreateService(out _, out _);
        service.CreateAccount(1, AccountType.Savings, "ACC-001");

        Assert.Throws<InvalidOperationException>(() => service.CreateAccount(2, AccountType.Checking, "ACC-001"));
    }

    [Fact]
    public void Deposit_ValidAmount_IncreasesBalance()
    {
        var service = CreateService(out _, out _);
        var account = service.CreateAccount(1, AccountType.Savings, "ACC-001");

        service.Deposit(account.Id, 500m);

        Assert.Equal(500m, service.GetBalance(account.Id));
    }

    [Fact]
    public void Deposit_ZeroAmount_ThrowsArgumentException()
    {
        var service = CreateService(out _, out _);
        var account = service.CreateAccount(1, AccountType.Savings, "ACC-001");

        Assert.Throws<ArgumentException>(() => service.Deposit(account.Id, 0));
    }

    [Fact]
    public void Deposit_NegativeAmount_ThrowsArgumentException()
    {
        var service = CreateService(out _, out _);
        var account = service.CreateAccount(1, AccountType.Savings, "ACC-001");

        Assert.Throws<ArgumentException>(() => service.Deposit(account.Id, -100));
    }

    [Fact]
    public void Deposit_NonExistentAccount_ThrowsInvalidOperationException()
    {
        var service = CreateService(out _, out _);

        Assert.Throws<InvalidOperationException>(() => service.Deposit(999, 100));
    }

    [Fact]
    public void Withdraw_ValidAmount_DecreasesBalance()
    {
        var service = CreateService(out _, out _);
        var account = service.CreateAccount(1, AccountType.Savings, "ACC-001");
        service.Deposit(account.Id, 1000m);

        service.Withdraw(account.Id, 300m);

        Assert.Equal(700m, service.GetBalance(account.Id));
    }

    [Fact]
    public void Withdraw_InsufficientFunds_ThrowsInvalidOperationException()
    {
        var service = CreateService(out _, out _);
        var account = service.CreateAccount(1, AccountType.Savings, "ACC-001");
        service.Deposit(account.Id, 100m);

        Assert.Throws<InvalidOperationException>(() => service.Withdraw(account.Id, 200m));
    }

    [Fact]
    public void Withdraw_ExactBalance_LeavesZero()
    {
        var service = CreateService(out _, out _);
        var account = service.CreateAccount(1, AccountType.Savings, "ACC-001");
        service.Deposit(account.Id, 500m);

        service.Withdraw(account.Id, 500m);

        Assert.Equal(0m, service.GetBalance(account.Id));
    }

    [Fact]
    public void Transfer_ValidAmount_MovesMoneyBetweenAccounts()
    {
        var service = CreateService(out _, out _);
        var from = service.CreateAccount(1, AccountType.Savings, "ACC-001");
        var to = service.CreateAccount(1, AccountType.Checking, "ACC-002");
        service.Deposit(from.Id, 1000m);

        service.Transfer(from.Id, to.Id, 400m);

        Assert.Equal(600m, service.GetBalance(from.Id));
        Assert.Equal(400m, service.GetBalance(to.Id));
    }

    [Fact]
    public void Transfer_InsufficientFunds_ThrowsInvalidOperationException()
    {
        var service = CreateService(out _, out _);
        var from = service.CreateAccount(1, AccountType.Savings, "ACC-001");
        var to = service.CreateAccount(1, AccountType.Checking, "ACC-002");
        service.Deposit(from.Id, 100m);

        Assert.Throws<InvalidOperationException>(() => service.Transfer(from.Id, to.Id, 500m));
    }

    [Fact]
    public void Transfer_SameAccount_ThrowsArgumentException()
    {
        var service = CreateService(out _, out _);
        var account = service.CreateAccount(1, AccountType.Savings, "ACC-001");
        service.Deposit(account.Id, 1000m);

        Assert.Throws<ArgumentException>(() => service.Transfer(account.Id, account.Id, 100m));
    }

    [Fact]
    public void Transfer_ZeroAmount_ThrowsArgumentException()
    {
        var service = CreateService(out _, out _);
        var from = service.CreateAccount(1, AccountType.Savings, "ACC-001");
        var to = service.CreateAccount(1, AccountType.Checking, "ACC-002");

        Assert.Throws<ArgumentException>(() => service.Transfer(from.Id, to.Id, 0));
    }

    [Fact]
    public void GetBalance_ExistingAccount_ReturnsCorrectBalance()
    {
        var service = CreateService(out _, out _);
        var account = service.CreateAccount(1, AccountType.Savings, "ACC-001");
        service.Deposit(account.Id, 750m);

        Assert.Equal(750m, service.GetBalance(account.Id));
    }

    [Fact]
    public void GetBalance_NonExistentAccount_ThrowsInvalidOperationException()
    {
        var service = CreateService(out _, out _);

        Assert.Throws<InvalidOperationException>(() => service.GetBalance(999));
    }

    [Fact]
    public void CalculateInterest_ValidInput_ReturnsCorrectAmount()
    {
        var service = CreateService(out _, out _);
        var account = service.CreateAccount(1, AccountType.Savings, "ACC-001");
        service.Deposit(account.Id, 10000m);

        // 10000 * (5/12/100) * 6 = 10000 * 0.004167 * 6 = 250.00
        var interest = service.CalculateInterest(account.Id, 5m, 6);

        Assert.Equal(250.00m, interest);
    }

    [Fact]
    public void CalculateInterest_ZeroBalance_ReturnsZero()
    {
        var service = CreateService(out _, out _);
        var account = service.CreateAccount(1, AccountType.Savings, "ACC-001");

        var interest = service.CalculateInterest(account.Id, 5m, 12);

        Assert.Equal(0m, interest);
    }

    [Fact]
    public void CalculateInterest_NegativeRate_ThrowsArgumentException()
    {
        var service = CreateService(out _, out _);
        var account = service.CreateAccount(1, AccountType.Savings, "ACC-001");

        Assert.Throws<ArgumentException>(() => service.CalculateInterest(account.Id, -5m, 6));
    }

    [Fact]
    public void GetAccountStatement_ReturnsTransactionsInDateRange()
    {
        var service = CreateService(out _, out var transactionRepo);
        var account = service.CreateAccount(1, AccountType.Savings, "ACC-001");
        service.Deposit(account.Id, 1000m);
        service.Withdraw(account.Id, 200m);

        var allTransactions = transactionRepo.GetByAccountId(account.Id);
        var minDate = allTransactions.Min(t => t.Date).AddSeconds(-1);
        var maxDate = allTransactions.Max(t => t.Date).AddSeconds(1);

        var statement = service.GetAccountStatement(account.Id, minDate, maxDate);

        Assert.Equal(2, statement.Count);
    }

    [Fact]
    public void GetAccountStatement_InvalidDateRange_ThrowsArgumentException()
    {
        var service = CreateService(out _, out _);
        var account = service.CreateAccount(1, AccountType.Savings, "ACC-001");

        Assert.Throws<ArgumentException>(() =>
            service.GetAccountStatement(account.Id, DateTime.Now, DateTime.Now.AddDays(-1)));
    }

    [Fact]
    public void GetTotalBalance_MultipleAccounts_ReturnsSumOfBalances()
    {
        var service = CreateService(out _, out _);
        var acc1 = service.CreateAccount(1, AccountType.Savings, "ACC-001");
        var acc2 = service.CreateAccount(1, AccountType.Checking, "ACC-002");
        service.Deposit(acc1.Id, 500m);
        service.Deposit(acc2.Id, 300m);

        var total = service.GetTotalBalance(1);

        Assert.Equal(800m, total);
    }

    [Fact]
    public void GetTotalBalance_NoAccounts_ReturnsZero()
    {
        var service = CreateService(out _, out _);

        var total = service.GetTotalBalance(999);

        Assert.Equal(0m, total);
    }

    [Fact]
    public void GetAccountsByCustomer_ReturnsOnlyCustomerAccounts()
    {
        var service = CreateService(out _, out _);
        service.CreateAccount(1, AccountType.Savings, "ACC-001");
        service.CreateAccount(1, AccountType.Checking, "ACC-002");
        service.CreateAccount(2, AccountType.Savings, "ACC-003");

        var accounts = service.GetAccountsByCustomer(1);

        Assert.Equal(2, accounts.Count);
    }

    [Fact]
    public void GetMonthlyReport_ContainsAccountNumber()
    {
        var service = CreateService(out _, out _);
        var account = service.CreateAccount(1, AccountType.Savings, "ACC-100");
        service.Deposit(account.Id, 500m);

        var report = service.GetMonthlyReport(account.Id, DateTime.Now.Month, DateTime.Now.Year);

        Assert.Contains("ACC-100", report);
    }

    [Fact]
    public void GetMonthlyReport_InvalidMonth_ThrowsArgumentException()
    {
        var service = CreateService(out _, out _);
        var account = service.CreateAccount(1, AccountType.Savings, "ACC-101");

        Assert.Throws<ArgumentException>(() =>
            service.GetMonthlyReport(account.Id, 13, DateTime.Now.Year));
    }

    [Fact]
    public void TransferWithFee_ValidData_DeductsFeeAndAmountFromSource()
    {
        var service = CreateService(out _, out _);
        var source = service.CreateAccount(1, AccountType.Checking, "ACC-200");
        var dest = service.CreateAccount(2, AccountType.Checking, "ACC-201");
        service.Deposit(source.Id, 1000m);

        service.TransferWithFee(source.Id, dest.Id, 500m, 2m);

        var sourceAccount = service.GetAccountsByCustomer(1).First();
        Assert.True(sourceAccount.Balance < 500m);
    }
}
