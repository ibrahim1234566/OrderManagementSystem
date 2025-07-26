using OrderManagementSystem.Data;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Repositories.Repository
{
    public class CustomerRepository : Repository<Customer>
    {
        public CustomerRepository(OrderManagementDbContext context) : base(context) { }
    }
}
