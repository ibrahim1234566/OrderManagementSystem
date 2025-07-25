using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Data;
using OrderManagementSystem.Models;
using OrderManagementSystem.Repositories.Interface;

namespace OrderManagementSystem.Repositories.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly OrderManagementDbContext _context;
        public CustomerRepository(OrderManagementDbContext context) => _context = context;

        public async Task<Customer> GetByIdAsync(int id) => await _context.Customers.FindAsync(id);

        public async Task AddAsync(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Order>> GetOrdersByCustomerIdAsync(int customerId)
        {
            return await _context.Orders.Where(o => o.CustomerId == customerId).Include(o => o.OrderItems).ToListAsync();
        }
    }
}
