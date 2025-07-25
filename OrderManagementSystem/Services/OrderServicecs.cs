using OrderManagementSystem.Data;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Services
{
    public class OrderServices
    {
        private readonly OrderManagementDbContext _context;
        public OrderServices(OrderManagementDbContext context) => _context = context;

        public async Task<Order> CreateOrderAsync(Order order)
        {
            decimal total = 0m;

            foreach (var item in order.OrderItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null || product.Stock < item.Quantity)
                    throw new Exception($"Insufficient stock for product ID {item.ProductId}");

                product.Stock -= item.Quantity;

                item.UnitPrice = product.Price;
                decimal subtotal = item.Quantity * product.Price;

                item.Discount = 0;
                total += subtotal;
            }

            decimal discountRate = total > 200 ? 0.10m : total > 100 ? 0.05m : 0;

            foreach (var item in order.OrderItems)
            {
                item.Discount = (item.Quantity * item.UnitPrice) * discountRate;
            }

            order.TotalAmount = order.OrderItems.Sum(i => (i.Quantity * i.UnitPrice) - i.Discount);
            order.OrderDate = DateTime.UtcNow;
            order.Status = "Pending";

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var invoice = new Invoice
            {
                OrderId = order.OrderId,
                InvoiceDate = DateTime.UtcNow,
                TotalAmount = order.TotalAmount
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return order;
        }
        public async Task UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) throw new Exception("Order not found");

            order.Status = newStatus;
            await _context.SaveChangesAsync();

            // Simulated Email Notification (mock)
            var customer = await _context.Customers.FindAsync(order.CustomerId);
            Console.WriteLine($"Email sent to {customer?.Email}: Order #{order.OrderId} status updated to {newStatus}.");
        }
    }
}
