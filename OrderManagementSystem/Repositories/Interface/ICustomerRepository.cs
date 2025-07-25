using OrderManagementSystem.Models;

namespace OrderManagementSystem.Repositories.Interface
{
    public interface ICustomerRepository
    {
        Task<Customer> GetByIdAsync(int id);
        Task AddAsync(Customer customer);
        Task<List<Order>> GetOrdersByCustomerIdAsync(int customerId);
    }
}
