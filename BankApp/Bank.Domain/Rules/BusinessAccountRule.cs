using Bank.Domain.Enums;

namespace Bank.Domain.Rules;

public class BusinessAccountRule : AccountRuleBase
{
    public override AccountType AccountType => AccountType.Business;
    public override decimal GetAnnualInterestRate() => 1.5m;
    public override decimal GetWithdrawalLimit() => 50000m;
    public override decimal GetMonthlyFee() => 99m;
}
