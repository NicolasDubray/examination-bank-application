// Examination: Nicolas Dubray
// Generated: 2026-04-02
// Domain: Bank

using Bank.Application.Services;
using Bank.Domain.Entities;
using Bank.Domain.Enums;
using Bank.Infrastructure.Factories;
using Bank.Infrastructure.Repositories;

namespace Bank.Tests;

public class AccountRuleServiceTests
{
    private AccountRuleService CreateService(out InMemoryAccountRepository accountRepo)
    {
        var factory = new AccountRuleFactory();
        accountRepo = new InMemoryAccountRepository();
        return new AccountRuleService(factory, accountRepo);
    }

    // --- GetInterestRate ---

    [Fact]
    public void GetInterestRate_Savings_Returns3Point5()
    {
        var service = CreateService(out _);
        Assert.Equal(3.5m, service.GetInterestRate(AccountType.Savings));
    }

    [Fact]
    public void GetInterestRate_Checking_Returns0Point5()
    {
        var service = CreateService(out _);
        Assert.Equal(0.5m, service.GetInterestRate(AccountType.Checking));
    }

    [Fact]
    public void GetInterestRate_Business_Returns1Point5()
    {
        var service = CreateService(out _);
        Assert.Equal(1.5m, service.GetInterestRate(AccountType.Business));
    }

    // --- GetMonthlyFee ---

    [Fact]
    public void GetMonthlyFee_Savings_ReturnsZero()
    {
        var service = CreateService(out _);
        Assert.Equal(0m, service.GetMonthlyFee(AccountType.Savings));
    }

    [Fact]
    public void GetMonthlyFee_Checking_Returns29()
    {
        var service = CreateService(out _);
        Assert.Equal(29m, service.GetMonthlyFee(AccountType.Checking));
    }

    [Fact]
    public void GetMonthlyFee_Business_Returns99()
    {
        var service = CreateService(out _);
        Assert.Equal(99m, service.GetMonthlyFee(AccountType.Business));
    }

    // --- GetWithdrawalLimit ---

    [Fact]
    public void GetWithdrawalLimit_Savings_Returns5000()
    {
        var service = CreateService(out _);
        Assert.Equal(5000m, service.GetWithdrawalLimit(AccountType.Savings));
    }

    [Fact]
    public void GetWithdrawalLimit_Checking_ReturnsZero()
    {
        var service = CreateService(out _);
        Assert.Equal(0m, service.GetWithdrawalLimit(AccountType.Checking));
    }

    [Fact]
    public void GetWithdrawalLimit_Business_Returns50000()
    {
        var service = CreateService(out _);
        Assert.Equal(50000m, service.GetWithdrawalLimit(AccountType.Business));
    }

    // --- CalculateMonthlyFees ---

    [Fact]
    public void CalculateMonthlyFees_MultipleAccounts_ReturnsTotalFees()
    {
        var service = CreateService(out var accountRepo);
        accountRepo.Add(new Account { Id = 1, CustomerId = 1, AccountType = AccountType.Savings, Balance = 10000 });
        accountRepo.Add(new Account { Id = 2, CustomerId = 1, AccountType = AccountType.Checking, Balance = 5000 });
        accountRepo.Add(new Account { Id = 3, CustomerId = 1, AccountType = AccountType.Business, Balance = 50000 });

        var total = service.CalculateMonthlyFees(1);

        Assert.Equal(128m, total); // 0 + 29 + 99
    }

    [Fact]
    public void CalculateMonthlyFees_NoAccounts_ReturnsZero()
    {
        var service = CreateService(out _);

        Assert.Equal(0m, service.CalculateMonthlyFees(999));
    }

    [Fact]
    public void CalculateMonthlyFees_OnlyReturnsCustomerAccounts()
    {
        var service = CreateService(out var accountRepo);
        accountRepo.Add(new Account { Id = 1, CustomerId = 1, AccountType = AccountType.Checking, Balance = 5000 });
        accountRepo.Add(new Account { Id = 2, CustomerId = 2, AccountType = AccountType.Business, Balance = 50000 });

        Assert.Equal(29m, service.CalculateMonthlyFees(1));
    }

    // --- IsWithdrawalAllowed ---

    [Fact]
    public void IsWithdrawalAllowed_SavingsWithinLimit_ReturnsTrue()
    {
        var service = CreateService(out var accountRepo);
        accountRepo.Add(new Account { Id = 1, CustomerId = 1, AccountType = AccountType.Savings, Balance = 10000 });

        Assert.True(service.IsWithdrawalAllowed(1, 3000));
    }

    [Fact]
    public void IsWithdrawalAllowed_SavingsExceedsLimit_ReturnsFalse()
    {
        var service = CreateService(out var accountRepo);
        accountRepo.Add(new Account { Id = 1, CustomerId = 1, AccountType = AccountType.Savings, Balance = 10000 });

        Assert.False(service.IsWithdrawalAllowed(1, 6000));
    }

    [Fact]
    public void IsWithdrawalAllowed_CheckingNoLimit_LargeWithdrawalAllowed()
    {
        var service = CreateService(out var accountRepo);
        accountRepo.Add(new Account { Id = 1, CustomerId = 1, AccountType = AccountType.Checking, Balance = 100000 });

        Assert.True(service.IsWithdrawalAllowed(1, 90000));
    }

    [Fact]
    public void IsWithdrawalAllowed_InsufficientBalance_ReturnsFalse()
    {
        var service = CreateService(out var accountRepo);
        accountRepo.Add(new Account { Id = 1, CustomerId = 1, AccountType = AccountType.Checking, Balance = 100 });

        Assert.False(service.IsWithdrawalAllowed(1, 500));
    }

    [Fact]
    public void IsWithdrawalAllowed_NegativeAmount_ThrowsArgumentException()
    {
        var service = CreateService(out var accountRepo);
        accountRepo.Add(new Account { Id = 1, CustomerId = 1, AccountType = AccountType.Savings, Balance = 10000 });

        Assert.Throws<ArgumentException>(() => service.IsWithdrawalAllowed(1, -100));
    }

    [Fact]
    public void IsWithdrawalAllowed_AccountNotFound_ThrowsInvalidOperationException()
    {
        var service = CreateService(out _);

        Assert.Throws<InvalidOperationException>(() => service.IsWithdrawalAllowed(999, 100));
    }

    // --- CalculateInterestByRule ---

    [Fact]
    public void CalculateInterestByRule_SavingsAccount_CalculatesCorrectly()
    {
        var service = CreateService(out var accountRepo);
        accountRepo.Add(new Account { Id = 1, CustomerId = 1, AccountType = AccountType.Savings, Balance = 10000 });

        // 3.5% annual / 12 = ~0.29167% monthly, * 10000 * 6 months
        var interest = service.CalculateInterestByRule(1, 6);
        var expected = Math.Round(10000m * (3.5m / 12m / 100m) * 6, 2);

        Assert.Equal(expected, interest);
    }

    [Fact]
    public void CalculateInterestByRule_CheckingAccount_CalculatesCorrectly()
    {
        var service = CreateService(out var accountRepo);
        accountRepo.Add(new Account { Id = 1, CustomerId = 1, AccountType = AccountType.Checking, Balance = 25000 });

        var interest = service.CalculateInterestByRule(1, 12);
        var expected = Math.Round(25000m * (0.5m / 12m / 100m) * 12, 2);

        Assert.Equal(expected, interest);
    }

    [Fact]
    public void CalculateInterestByRule_InvalidMonths_ThrowsArgumentException()
    {
        var service = CreateService(out var accountRepo);
        accountRepo.Add(new Account { Id = 1, CustomerId = 1, AccountType = AccountType.Savings, Balance = 10000 });

        Assert.Throws<ArgumentException>(() => service.CalculateInterestByRule(1, 0));
    }

    [Fact]
    public void CalculateInterestByRule_AccountNotFound_ThrowsInvalidOperationException()
    {
        var service = CreateService(out _);

        Assert.Throws<InvalidOperationException>(() => service.CalculateInterestByRule(999, 6));
    }

    // --- GetAccountSummary ---

    [Fact]
    public void GetAccountSummary_ReturnsCorrectFormat()
    {
        var service = CreateService(out var accountRepo);
        accountRepo.Add(new Account
        {
            Id = 1, CustomerId = 1, AccountNumber = "ACC-001",
            AccountType = AccountType.Savings, Balance = 12345.67m
        });

        var summary = service.GetAccountSummary(1);

        Assert.Contains("ACC-001", summary);
        Assert.Contains("Savings", summary);
        Assert.Contains("12345,67", summary);
        Assert.Contains("3,50%", summary);
        Assert.Contains("0,00", summary);
    }

    [Fact]
    public void GetAccountSummary_BusinessAccount_ShowsCorrectFee()
    {
        var service = CreateService(out var accountRepo);
        accountRepo.Add(new Account
        {
            Id = 1, CustomerId = 1, AccountNumber = "BIZ-001",
            AccountType = AccountType.Business, Balance = 50000
        });

        var summary = service.GetAccountSummary(1);

        Assert.Contains("99,00", summary);
        Assert.Contains("1,50%", summary);
    }

    [Fact]
    public void GetAccountSummary_AccountNotFound_ThrowsInvalidOperationException()
    {
        var service = CreateService(out _);

        Assert.Throws<InvalidOperationException>(() => service.GetAccountSummary(999));
    }

    // --- Factory Tests ---

    [Fact]
    public void Factory_CreatesSavingsRule_WithCorrectValues()
    {
        var factory = new AccountRuleFactory();
        var rule = factory.CreateRule(AccountType.Savings);

        Assert.Equal(3.5m, rule.GetAnnualInterestRate());
        Assert.Equal(5000m, rule.GetWithdrawalLimit());
        Assert.Equal(0m, rule.GetMonthlyFee());
    }

    [Fact]
    public void Factory_CreatesCheckingRule_WithCorrectValues()
    {
        var factory = new AccountRuleFactory();
        var rule = factory.CreateRule(AccountType.Checking);

        Assert.Equal(0.5m, rule.GetAnnualInterestRate());
        Assert.Equal(0m, rule.GetWithdrawalLimit());
        Assert.Equal(29m, rule.GetMonthlyFee());
    }

    [Fact]
    public void Factory_CreatesBusinessRule_WithCorrectValues()
    {
        var factory = new AccountRuleFactory();
        var rule = factory.CreateRule(AccountType.Business);

        Assert.Equal(1.5m, rule.GetAnnualInterestRate());
        Assert.Equal(50000m, rule.GetWithdrawalLimit());
        Assert.Equal(99m, rule.GetMonthlyFee());
    }

    [Fact]
    public void CalculateCompoundInterest_IsGreaterThanSimpleInterest()
    {
        var service = CreateService(out var accountRepo);
        var account = new Bank.Domain.Entities.Account
        {
            Id = 1,
            AccountNumber = "ACC-300",
            CustomerId = 1,
            Balance = 1000m,
            AccountType = AccountType.Savings
        };
        accountRepo.Add(account);

        var annualRate = 12m;
        var months = 12;
        var compound = service.CalculateCompoundInterest(account.Id, annualRate, months);
        var simple = account.Balance * annualRate / 100m;

        Assert.True(compound > simple);
    }

    [Fact]
    public void CalculateCompoundInterest_ZeroRate_ReturnsZero()
    {
        var service = CreateService(out var accountRepo);
        var account = new Bank.Domain.Entities.Account
        {
            Id = 2,
            AccountNumber = "ACC-301",
            CustomerId = 1,
            Balance = 500m,
            AccountType = AccountType.Savings
        };
        accountRepo.Add(account);

        var interest = service.CalculateCompoundInterest(account.Id, 0m, 12);

        Assert.Equal(0m, interest);
    }
}
