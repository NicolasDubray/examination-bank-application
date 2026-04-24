using Bank.Domain.Entities;
using Bank.Domain.Enums;
using Bank.Domain.Interfaces;

namespace Bank.Infrastructure.Seed;

public static class Seeder
{
    public static void SeedAll(
        ICustomerRepository customerRepo,
        IAccountRepository accountRepo,
        ITransactionRepository transactionRepo)
    {
        SeedCustomers(customerRepo);
        SeedAccounts(accountRepo);
        SeedTransactions(transactionRepo);
    }

    public static void SeedCustomers(ICustomerRepository repo)
    {
        repo.Add(new Customer { Id = 1, FirstName = "Anna", LastName = "Svensson" });
        repo.Add(new Customer { Id = 2, FirstName = "Erik", LastName = "Johansson" });
    }

    public static void SeedAccounts(IAccountRepository repo)
    {
        repo.Add(new Account
        {
            Id = 1,
            CustomerId = 1,
            AccountNumber = "SE1001",
            Balance = 15000,
            AccountType = AccountType.Savings
        });

        repo.Add(new Account
        {
            Id = 2,
            CustomerId = 1,
            AccountNumber = "SE1002",
            Balance = 3500,
            AccountType = AccountType.Checking
        });

        repo.Add(new Account
        {
            Id = 3,
            CustomerId = 1,
            AccountNumber = "SE1003",
            Balance = 42000,
            AccountType = AccountType.Business
        });

        repo.Add(new Account
        {
            Id = 4,
            CustomerId = 2,
            AccountNumber = "SE2001",
            Balance = 8000,
            AccountType = AccountType.Savings
        });

        repo.Add(new Account
        {
            Id = 5,
            CustomerId = 2,
            AccountNumber = "SE2002",
            Balance = 1200,
            AccountType = AccountType.Checking
        });
    }

    public static void SeedTransactions(ITransactionRepository repo)
    {
        repo.Add(new Transaction
        {
            AccountId = 1,
            Date = DateTime.Now.AddDays(-1),
            Amount = 2000,
            TransactionType = TransactionType.Deposit,
            Description = "Salary"
        });

        repo.Add(new Transaction
        {
            AccountId = 1,
            Date = DateTime.Now.AddDays(-2),
            Amount = -300,
            TransactionType = TransactionType.Withdrawal,
            Description = "Groceries"
        });

        repo.Add(new Transaction
        {
            AccountId = 1,
            Date = DateTime.Now.AddDays(-3),
            Amount = -500,
            TransactionType = TransactionType.Transfer,
            Description = "Transfer to savings"
        });

        repo.Add(new Transaction
        {
            AccountId = 2,
            Date = DateTime.Now.AddDays(-1),
            Amount = 1000,
            TransactionType = TransactionType.Deposit,
            Description = "Freelance work"
        });

        repo.Add(new Transaction
        {
            AccountId = 2,
            Date = DateTime.Now.AddDays(-2),
            Amount = -150,
            TransactionType = TransactionType.Withdrawal,
            Description = "Restaurant"
        });

        repo.Add(new Transaction
        {
            AccountId = 2,
            Date = DateTime.Now.AddDays(-3),
            Amount = -200,
            TransactionType = TransactionType.Transfer,
            Description = "Transfer to checking"
        });

        repo.Add(new Transaction
        {
            AccountId = 3,
            Date = DateTime.Now.AddDays(-1),
            Amount = 5000,
            TransactionType = TransactionType.Deposit,
            Description = "Client payment"
        });

        repo.Add(new Transaction
        {
            AccountId = 3,
            Date = DateTime.Now.AddDays(-2),
            Amount = -1200,
            TransactionType = TransactionType.Withdrawal,
            Description = "Office supplies"
        });

        repo.Add(new Transaction
        {
            AccountId = 3,
            Date = DateTime.Now.AddDays(-3),
            Amount = -2000,
            TransactionType = TransactionType.Transfer,
            Description = "Transfer to payroll"
        });

        repo.Add(new Transaction
        {
            AccountId = 4,
            Date = DateTime.Now.AddDays(-1),
            Amount = 1500,
            TransactionType = TransactionType.Deposit,
            Description = "Monthly savings"
        });

        repo.Add(new Transaction
        {
            AccountId = 4,
            Date = DateTime.Now.AddDays(-2),
            Amount = -100,
            TransactionType = TransactionType.Withdrawal,
            Description = "ATM withdrawal"
        });

        repo.Add(new Transaction
        {
            AccountId = 4,
            Date = DateTime.Now.AddDays(-3),
            Amount = -250,
            TransactionType = TransactionType.Transfer,
            Description = "Transfer to checking"
        });

        repo.Add(new Transaction
        {
            AccountId = 5,
            Date = DateTime.Now.AddDays(-1),
            Amount = 800,
            TransactionType = TransactionType.Deposit,
            Description = "Refund"
        });

        repo.Add(new Transaction
        {
            AccountId = 5,
            Date = DateTime.Now.AddDays(-2),
            Amount = -60,
            TransactionType = TransactionType.Withdrawal,
            Description = "Coffee & snacks"
        });

        repo.Add(new Transaction
        {
            AccountId = 5,
            Date = DateTime.Now.AddDays(-3),
            Amount = -120,
            TransactionType = TransactionType.Transfer,
            Description = "Transfer to savings"
        });
    }
}
