using Bank.Domain.Enums;

namespace Bank.Domain.Entities;

public class Transaction
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public decimal Amount { get; set; }
    public TransactionType TransactionType { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
}
