using Bank.Application.DTOs;
using Bank.Domain.Entities;
using Bank.Domain.Interfaces;

namespace Bank.Application.Services;

public class CustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IAccountRepository _accountRepository;
    private int _nextCustomerId = 1;

    public CustomerService(ICustomerRepository customerRepository, IAccountRepository accountRepository)
    {
        _customerRepository = customerRepository;
        _accountRepository = accountRepository;
    }

    // BUG_TARGET: CreateCustomer
    public Customer CreateCustomer(string firstName, string lastName, string email)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty.");
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty.");
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new ArgumentException("A valid email is required.");

        var customer = new Customer
        {
            Id = _nextCustomerId++,
            FirstName = firstName,
            LastName = lastName,
            Email = email
        };

        _customerRepository.Add(customer);
        return customer;
    }

    public Customer? GetCustomerById(int id)
    {
        return _customerRepository.GetById(id);
    }

    public List<Customer> GetAllCustomers()
    {
        return _customerRepository.GetAll();
    }

    // MISSING_TARGET: GetCustomerFullName
    public string GetCustomerFullName(int customerId)
    {
        var customer = _customerRepository.GetById(customerId);
        if (customer == null)
            throw new InvalidOperationException("Customer not found.");

        return customer.FullName;
    }

    // BUG_TARGET: GetCustomerAccountCount
    public int GetCustomerAccountCount(int customerId)
    {
        var customer = _customerRepository.GetById(customerId);
        if (customer == null)
            throw new InvalidOperationException("Customer not found.");

        return _accountRepository.GetByCustomerId(customerId).Count;
    }

    // MISSING_TARGET: GetCustomerAccountSummary
    public string GetCustomerAccountSummary(int customerId)
    {
        var customer = _customerRepository.GetById(customerId);
        if (customer == null)
            throw new InvalidOperationException("Customer not found.");

        var accounts = _accountRepository.GetByCustomerId(customerId);
        var totalBalance = accounts.Sum(a => a.Balance);

        var typeSummary = accounts
            .GroupBy(a => a.AccountType)
            .Select(g => $"{g.Key}: {g.Count()}")
            .ToList();
        var typeStr = typeSummary.Count > 0 ? string.Join(", ", typeSummary) : "Inga konton";

        return $"{customer.FullName}: {accounts.Count} konton ({typeStr}), Totalt saldo: {totalBalance:F2}";
    }

    public List<CustomerSelectionDto> GetCustomerSelections()
    {
        return _customerRepository.GetAll()
            .Select(c => new CustomerSelectionDto(c.Id, c.FullName))
            .ToList();
    }

    public CustomerOverviewDto GetCustomerOverview(int customerId)
    {
        var customer = _customerRepository.GetById(customerId)
            ?? throw new InvalidOperationException("Customer not found.");

        var accounts = _accountRepository.GetByCustomerId(customerId);

        return new CustomerOverviewDto(
            customer.Id,
            customer.FullName,
            accounts.Select(a => new AccountSummaryDto(
                a.Id,
                a.AccountNumber,
                a.AccountType.ToString(),
                a.Balance
            )).ToList()
        );
    }
}
