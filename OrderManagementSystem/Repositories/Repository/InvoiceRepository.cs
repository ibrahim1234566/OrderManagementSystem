using OrderManagementSystem.Data;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Repositories.Repository
{
    public class InvoiceRepository : Repository<Invoice>
    {
        public InvoiceRepository(OrderManagementDbContext context) : base(context) { }
        
    }
}
