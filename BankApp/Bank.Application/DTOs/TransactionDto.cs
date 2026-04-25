using System;
using System.Collections.Generic;
using System.Text;

namespace Bank.Application.DTOs
{
    public record TransactionDto(
        DateTime Date,
        decimal Amount,
        string Type,        
        string Description
    );
}
