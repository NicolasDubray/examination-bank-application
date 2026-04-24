using Bank.Domain.Entities;
using Bank.Domain.Interfaces;

namespace Bank.Infrastructure.Repositories;

public class InMemoryCustomerRepository : ICustomerRepository
{
    private readonly List<Customer> _customers = new();

    public Customer? GetById(int id) => _customers.FirstOrDefault(c => c.Id == id);
    public List<Customer> GetAll() => _customers.ToList();
    public void Add(Customer customer) => _customers.Add(customer);

    // BUG_TARGET: Update
    public void Update(Customer customer)
    {
        var index = _customers.FindIndex(c => c.Id == customer.Id);
        if (index >= 0)
            _customers[index] = customer;
    }

    public int Count() => _customers.Count;
}
