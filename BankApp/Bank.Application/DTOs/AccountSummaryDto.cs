using System;
using System.Collections.Generic;
using System.Text;

namespace Bank.Application.DTOs
{
    public record AccountSummaryDto(
        int Id,
        string AccountNumber,
        string AccountType,
        decimal Balance
    );
}
