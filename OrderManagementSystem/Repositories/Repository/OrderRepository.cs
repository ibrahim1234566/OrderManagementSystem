using OrderManagementSystem.Data;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Repositories.Repository
{
    public class OrderRepository : Repository<Order>
    {
        public OrderRepository(OrderManagementDbContext context) : base(context) { }
    }
}
