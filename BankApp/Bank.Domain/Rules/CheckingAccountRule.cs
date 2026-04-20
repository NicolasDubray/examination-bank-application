using Bank.Domain.Enums;

namespace Bank.Domain.Rules;

public class CheckingAccountRule : AccountRuleBase
{
    public override AccountType AccountType => AccountType.Checking;
    public override decimal GetAnnualInterestRate() => 0.5m;
    public override decimal GetWithdrawalLimit() => 0m;
    public override decimal GetMonthlyFee() => 29m;
}
