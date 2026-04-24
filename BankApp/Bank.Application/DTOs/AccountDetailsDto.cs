using System;
using System.Collections.Generic;
using System.Text;

namespace Bank.Application.DTOs
{
    public record AccountDetailsDto(
        int Id,
        string AccountNumber,
        decimal Balance,
        string AccountType,
        decimal InterestRate,
        decimal MonthlyFee,
        decimal WithdrawalLimit
    );
}
