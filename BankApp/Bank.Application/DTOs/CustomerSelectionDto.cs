using System;
using System.Collections.Generic;
using System.Text;

namespace Bank.Application.DTOs
{
    public record CustomerSelectionDto(
        int Id,
        string FullName
    );
}
