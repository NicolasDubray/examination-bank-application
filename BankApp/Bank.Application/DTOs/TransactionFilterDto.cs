using System;
using System.Collections.Generic;
using System.Text;

namespace Bank.Application.DTOs
{
    public record TransactionFilterDto(
        DateTime? FromDate,
        DateTime? ToDate,
        string? TransactionType
    );
}
