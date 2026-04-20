// Examination: Nicolas Dubray
// Generated: 2026-04-02
// Domain: Bank

using Bank.Application.Services;
using Bank.Domain.Enums;
using Bank.Infrastructure.Repositories;

namespace Bank.Tests;

public class CustomerServiceTests
{
    private CustomerService CreateService()
    {
        var customerRepo = new InMemoryCustomerRepository();
        var accountRepo = new InMemoryAccountRepository();
        return new CustomerService(customerRepo, accountRepo);
    }

    private (CustomerService customerService, AccountService accountService) CreateServicesWithAccounts()
    {
        var customerRepo = new InMemoryCustomerRepository();
        var accountRepo = new InMemoryAccountRepository();
        var transactionRepo = new InMemoryTransactionRepository();
        var customerService = new CustomerService(customerRepo, accountRepo);
        var accountService = new AccountService(accountRepo, transactionRepo);
        return (customerService, accountService);
    }

    [Fact]
    public void CreateCustomer_ValidInput_ReturnsCustomer()
    {
        var service = CreateService();

        var customer = service.CreateCustomer("Anna", "Svensson", "anna@test.com");

        Assert.NotNull(customer);
        Assert.Equal("Anna", customer.FirstName);
        Assert.Equal("Svensson", customer.LastName);
        Assert.Equal("anna@test.com", customer.Email);
    }

    [Fact]
    public void CreateCustomer_EmptyFirstName_ThrowsArgumentException()
    {
        var service = CreateService();

        Assert.Throws<ArgumentException>(() => service.CreateCustomer("", "Svensson", "anna@test.com"));
    }

    [Fact]
    public void CreateCustomer_EmptyLastName_ThrowsArgumentException()
    {
        var service = CreateService();

        Assert.Throws<ArgumentException>(() => service.CreateCustomer("Anna", "", "anna@test.com"));
    }

    [Fact]
    public void CreateCustomer_InvalidEmail_ThrowsArgumentException()
    {
        var service = CreateService();

        Assert.Throws<ArgumentException>(() => service.CreateCustomer("Anna", "Svensson", "invalid-email"));
    }

    [Fact]
    public void GetCustomerById_ExistingCustomer_ReturnsCustomer()
    {
        var service = CreateService();
        var created = service.CreateCustomer("Anna", "Svensson", "anna@test.com");

        var found = service.GetCustomerById(created.Id);

        Assert.NotNull(found);
        Assert.Equal(created.Id, found.Id);
    }

    [Fact]
    public void GetCustomerById_NonExistent_ReturnsNull()
    {
        var service = CreateService();

        var found = service.GetCustomerById(999);

        Assert.Null(found);
    }

    [Fact]
    public void GetCustomerFullName_ReturnsFormattedName()
    {
        var service = CreateService();
        var customer = service.CreateCustomer("Anna", "Svensson", "anna@test.com");

        var fullName = service.GetCustomerFullName(customer.Id);

        Assert.Equal("Anna Svensson", fullName);
    }

    [Fact]
    public void GetCustomerFullName_NonExistent_ThrowsInvalidOperationException()
    {
        var service = CreateService();

        Assert.Throws<InvalidOperationException>(() => service.GetCustomerFullName(999));
    }

    [Fact]
    public void GetCustomerAccountCount_ReturnsCorrectCount()
    {
        var (customerService, accountService) = CreateServicesWithAccounts();
        var customer = customerService.CreateCustomer("Anna", "Svensson", "anna@test.com");
        accountService.CreateAccount(customer.Id, AccountType.Savings, "ACC-001");
        accountService.CreateAccount(customer.Id, AccountType.Checking, "ACC-002");

        var count = customerService.GetCustomerAccountCount(customer.Id);

        Assert.Equal(2, count);
    }

    [Fact]
    public void GetCustomerAccountCount_NoAccounts_ReturnsZero()
    {
        var (customerService, _) = CreateServicesWithAccounts();
        var customer = customerService.CreateCustomer("Anna", "Svensson", "anna@test.com");

        var count = customerService.GetCustomerAccountCount(customer.Id);

        Assert.Equal(0, count);
    }

    [Fact]
    public void GetAllCustomers_ReturnsAllCustomers()
    {
        var service = CreateService();
        service.CreateCustomer("Anna", "Svensson", "anna@test.com");
        service.CreateCustomer("Erik", "Johansson", "erik@test.com");

        var all = service.GetAllCustomers();

        Assert.Equal(2, all.Count);
    }
}
