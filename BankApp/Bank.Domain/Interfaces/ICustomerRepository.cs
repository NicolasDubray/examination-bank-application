using Bank.Domain.Entities;

namespace Bank.Domain.Interfaces;

public interface ICustomerRepository
{
    Customer? GetById(int id);
    List<Customer> GetAll();
    void Add(Customer customer);
    void Update(Customer customer);
    int Count();
}
