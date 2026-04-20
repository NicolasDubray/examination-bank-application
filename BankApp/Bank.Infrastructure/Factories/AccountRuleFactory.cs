using Bank.Domain.Enums;
using Bank.Domain.Interfaces;
using Bank.Domain.Rules;

namespace Bank.Infrastructure.Factories;

public class AccountRuleFactory : IAccountRuleFactory
{
    // BUG_TARGET: CreateRule
    public AccountRuleBase CreateRule(AccountType accountType) => accountType switch
    {
        AccountType.Savings => new SavingsAccountRule(),
        AccountType.Checking => new CheckingAccountRule(),
        AccountType.Business => new BusinessAccountRule(),
        _ => throw new ArgumentOutOfRangeException(nameof(accountType))
    };
}
