using OrderManagementSystem.Models;

namespace OrderManagementSystem.Repositories.Interface
{
    public interface IOrderRepository
    {
        Task<Order> GetByIdAsync(int id);
        Task<List<Order>> GetAllAsync();
        Task AddAsync(Order order);
        Task UpdateStatusAsync(int orderId, string status);
    }
}
