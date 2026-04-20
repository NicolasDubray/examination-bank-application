using Bank.Domain.Enums;

namespace Bank.Domain.Rules;

public abstract class AccountRuleBase
{
    public abstract AccountType AccountType { get; }
    public abstract decimal GetAnnualInterestRate();
    public abstract decimal GetWithdrawalLimit();
    public abstract decimal GetMonthlyFee();

    public decimal CalculateInterestForBalance(decimal balance, int months)
    {
        var monthlyRate = GetAnnualInterestRate() / 12m / 100m;
        return Math.Round(balance * monthlyRate * months, 2);
    }

    public bool IsWithdrawalAllowed(decimal balance, decimal amount)
    {
        var limit = GetWithdrawalLimit();
        if (limit > 0 && amount > limit)
            return false;
        return balance >= amount;
    }
}
