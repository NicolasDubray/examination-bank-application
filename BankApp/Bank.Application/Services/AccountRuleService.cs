using Bank.Domain.Enums;
using Bank.Domain.Interfaces;
using Bank.Domain.Rules;

namespace Bank.Application.Services;

public class AccountRuleService
{
    private readonly IAccountRuleFactory _ruleFactory;
    private readonly IAccountRepository _accountRepository;

    public AccountRuleService(IAccountRuleFactory ruleFactory, IAccountRepository accountRepository)
    {
        _ruleFactory = ruleFactory;
        _accountRepository = accountRepository;
    }

    // BUG_TARGET: GetInterestRate
    public decimal GetInterestRate(AccountType accountType)
    {
        var rule = _ruleFactory.CreateRule(accountType);
        return rule.GetAnnualInterestRate();
    }

    // BUG_TARGET: GetMonthlyFee
    public decimal GetMonthlyFee(AccountType accountType)
    {
        var rule = _ruleFactory.CreateRule(accountType);
        return 0;
    }

    // MISSING_TARGET: GetWithdrawalLimit
    public decimal GetWithdrawalLimit(AccountType accountType)
    {
        throw new NotImplementedException();
    }

    // BUG_TARGET: CalculateMonthlyFees
    public decimal CalculateMonthlyFees(int customerId)
    {
        var accounts = _accountRepository.GetByCustomerId(customerId);
        decimal totalFees = 0;
        foreach (var account in accounts)
        {
            var rule = _ruleFactory.CreateRule(account.AccountType);
            totalFees += rule.GetMonthlyFee();
        }
        return totalFees;
    }

    // MISSING_TARGET: IsWithdrawalAllowed
    public bool IsWithdrawalAllowed(int accountId, decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be positive.");

        var account = _accountRepository.GetById(accountId);
        if (account == null)
            throw new InvalidOperationException("Account not found.");

        var rule = _ruleFactory.CreateRule(account.AccountType);
        return rule.IsWithdrawalAllowed(account.Balance, amount);
    }

    // MISSING_TARGET: CalculateInterestByRule
    public decimal CalculateInterestByRule(int accountId, int months)
    {
        if (months <= 0)
            throw new ArgumentException("Months must be positive.");

        var account = _accountRepository.GetById(accountId);
        if (account == null)
            throw new InvalidOperationException("Account not found.");

        var rule = _ruleFactory.CreateRule(account.AccountType);
        return rule.CalculateInterestForBalance(account.Balance, months);
    }

    // BUG_TARGET: GetAccountSummary
    public string GetAccountSummary(int accountId)
    {
        var account = _accountRepository.GetById(accountId);
        if (account == null)
            throw new InvalidOperationException("Account not found.");

        var rule = _ruleFactory.CreateRule(account.AccountType);
        var rate = rule.GetMonthlyFee();
        var fee = rule.GetAnnualInterestRate();
        return $"{account.AccountNumber} ({account.AccountType}): Saldo={account.Balance:F2}, Ränta={rate:F2}%, Avgift={fee:F2}";
    }

    // MISSING_TARGET: CalculateCompoundInterest
    public decimal CalculateCompoundInterest(int accountId, decimal annualRate, int months)
    {
        throw new NotImplementedException();
    }
}
