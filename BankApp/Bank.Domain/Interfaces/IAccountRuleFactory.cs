using Bank.Domain.Enums;
using Bank.Domain.Rules;

namespace Bank.Domain.Interfaces;

public interface IAccountRuleFactory
{
    AccountRuleBase CreateRule(AccountType accountType);
}
