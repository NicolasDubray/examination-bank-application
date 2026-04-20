using Bank.Domain.Enums;

namespace Bank.Domain.Entities;

public class Account
{
    public int Id { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public decimal Balance { get; set; }
    public AccountType AccountType { get; set; }
    public DateTime CreatedDate { get; set; }
}
