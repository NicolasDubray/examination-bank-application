using Bank.Application.Services;
using Bank.Domain.Enums;
using Bank.Infrastructure.Factories;
using Bank.Infrastructure.Repositories;

namespace Bank.Console;

public class Program
{
    public static void Main(string[] args)
    {
        var customerRepo = new InMemoryCustomerRepository();
        var accountRepo = new InMemoryAccountRepository();
        var transactionRepo = new InMemoryTransactionRepository();

        var accountRuleFactory = new AccountRuleFactory();
        var accountRuleService = new AccountRuleService(accountRuleFactory, accountRepo);
        var customerService = new CustomerService(customerRepo, accountRepo);
        var accountService = new AccountService(accountRepo, transactionRepo, accountRuleService);
        var transactionService = new TransactionService(transactionRepo);

        bool running = true;
        while (running)
        {
            System.Console.WriteLine("\n=== Bank Application ===");
            System.Console.WriteLine("1. Create Customer");
            System.Console.WriteLine("2. Create Account");
            System.Console.WriteLine("3. Deposit");
            System.Console.WriteLine("4. Withdraw");
            System.Console.WriteLine("5. Transfer");
            System.Console.WriteLine("6. View Balance");
            System.Console.WriteLine("7. View Account Statement");
            System.Console.WriteLine("8. Calculate Interest");
            System.Console.WriteLine("9. View Transactions");
            System.Console.WriteLine("0. Exit");
            System.Console.Write("Choose: ");

            var choice = System.Console.ReadLine();
            try
            {
                switch (choice)
                {
                    case "1":
                        System.Console.Write("First name: ");
                        var firstName = System.Console.ReadLine()!;
                        System.Console.Write("Last name: ");
                        var lastName = System.Console.ReadLine()!;
                        System.Console.Write("Email: ");
                        var email = System.Console.ReadLine()!;
                        var customer = customerService.CreateCustomer(firstName, lastName, email);
                        System.Console.WriteLine($"Customer created with ID: {customer.Id}");
                        break;

                    case "2":
                        System.Console.Write("Customer ID: ");
                        var custId = int.Parse(System.Console.ReadLine()!);
                        System.Console.Write("Account number: ");
                        var accNum = System.Console.ReadLine()!;
                        System.Console.Write("Type (0=Savings, 1=Checking, 2=Business): ");
                        var accType = (AccountType)int.Parse(System.Console.ReadLine()!);
                        var account = accountService.CreateAccount(custId, accType, accNum);
                        System.Console.WriteLine($"Account created with ID: {account.Id}");
                        break;

                    case "3":
                        System.Console.Write("Account ID: ");
                        var depAccId = int.Parse(System.Console.ReadLine()!);
                        System.Console.Write("Amount: ");
                        var depAmount = decimal.Parse(System.Console.ReadLine()!);
                        accountService.Deposit(depAccId, depAmount);
                        System.Console.WriteLine("Deposit successful.");
                        break;

                    case "4":
                        System.Console.Write("Account ID: ");
                        var wdAccId = int.Parse(System.Console.ReadLine()!);
                        System.Console.Write("Amount: ");
                        var wdAmount = decimal.Parse(System.Console.ReadLine()!);
                        accountService.Withdraw(wdAccId, wdAmount);
                        System.Console.WriteLine("Withdrawal successful.");
                        break;

                    case "5":
                        System.Console.Write("From Account ID: ");
                        var fromId = int.Parse(System.Console.ReadLine()!);
                        System.Console.Write("To Account ID: ");
                        var toId = int.Parse(System.Console.ReadLine()!);
                        System.Console.Write("Amount: ");
                        var trAmount = decimal.Parse(System.Console.ReadLine()!);
                        accountService.Transfer(fromId, toId, trAmount);
                        System.Console.WriteLine("Transfer successful.");
                        break;

                    case "6":
                        System.Console.Write("Account ID: ");
                        var balAccId = int.Parse(System.Console.ReadLine()!);
                        var balance = accountService.GetBalance(balAccId);
                        System.Console.WriteLine($"Balance: {balance:C}");
                        break;

                    case "7":
                        System.Console.Write("Account ID: ");
                        var stmtAccId = int.Parse(System.Console.ReadLine()!);
                        System.Console.Write("From date (yyyy-MM-dd): ");
                        var from = DateTime.Parse(System.Console.ReadLine()!);
                        System.Console.Write("To date (yyyy-MM-dd): ");
                        var to = DateTime.Parse(System.Console.ReadLine()!);
                        var stmt = accountService.GetAccountStatement(stmtAccId, from, to);
                        foreach (var t in stmt)
                            System.Console.WriteLine($"  {t.Date:yyyy-MM-dd} | {t.TransactionType} | {t.Amount:C} | {t.Description}");
                        break;

                    case "8":
                        System.Console.Write("Account ID: ");
                        var intAccId = int.Parse(System.Console.ReadLine()!);
                        System.Console.Write("Annual rate (%): ");
                        var rate = decimal.Parse(System.Console.ReadLine()!);
                        System.Console.Write("Months: ");
                        var months = int.Parse(System.Console.ReadLine()!);
                        var interest = accountService.CalculateInterest(intAccId, rate, months);
                        System.Console.WriteLine($"Calculated interest: {interest:C}");
                        break;

                    case "9":
                        System.Console.Write("Account ID: ");
                        var txAccId = int.Parse(System.Console.ReadLine()!);
                        var transactions = transactionService.GetTransactionsByAccountId(txAccId);
                        foreach (var t in transactions)
                            System.Console.WriteLine($"  {t.Date:yyyy-MM-dd} | {t.TransactionType} | {t.Amount:C} | {t.Description}");
                        break;

                    case "0":
                        running = false;
                        break;

                    default:
                        System.Console.WriteLine("Invalid choice.");
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
