using Bank.Domain.Enums;

namespace Bank.Domain.Rules;

public class SavingsAccountRule : AccountRuleBase
{
    public override AccountType AccountType => AccountType.Savings;
    public override decimal GetAnnualInterestRate() => 3.5m;
    public override decimal GetWithdrawalLimit() => 5000m;
    public override decimal GetMonthlyFee() => 0m;
}
