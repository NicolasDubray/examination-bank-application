using System;
using System.Collections.Generic;
using System.Text;

namespace Bank.Application.DTOs
{
    public record CustomerOverviewDto(
        int Id,
        string FullName,
        List<AccountSummaryDto> Accounts
    );
}
