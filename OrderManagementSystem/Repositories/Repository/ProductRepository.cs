using OrderManagementSystem.Data;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Repositories.Repository
{
    public class ProductRepository : Repository<Product>
    {
        public ProductRepository(OrderManagementDbContext context) : base(context) { }
    }
}
