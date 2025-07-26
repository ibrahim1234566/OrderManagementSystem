using OrderManagementSystem.Data;
using OrderManagementSystem.Models;
using OrderManagementSystem.OrderDTO;
using OrderManagementSystem.Repositories.Repository;

namespace OrderManagementSystem.Services
{
    public class OrderServices
    {
        private readonly OrderRepository _orderRepo;
        private readonly ProductRepository _productRepo;
        private readonly InvoiceRepository _invoiceRepo;
        private readonly CustomerRepository _customerRepo;

        public OrderServices(
            OrderRepository orderRepo,
            ProductRepository productRepo,
            InvoiceRepository invoiceRepo,
            CustomerRepository customerRepo)
        {
            _orderRepo = orderRepo;
            _productRepo = productRepo;
            _invoiceRepo = invoiceRepo;
            _customerRepo = customerRepo;
        }

        public async Task<OrderResponseDto> CreateOrderAsync(OrderDto dto)
        {
            if (dto.OrderItems == null || !dto.OrderItems.Any())
                throw new Exception("Order must have at least one item.");

            var order = new Order
            {
                CustomerId = dto.CustomerId,
                PaymentMethod = dto.PaymentMethod,
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                OrderItems = new List<OrderItem>()
            };

            decimal total = 0m;

            foreach (var itemDto in dto.OrderItems)
            {
                var product = await _productRepo.GetByIdAsync(itemDto.ProductId);
                if (product == null)
                    throw new Exception($"Product with ID {itemDto.ProductId} not found.");
                if (product.Stock < itemDto.Quantity)
                    throw new Exception($"Insufficient stock for product ID {itemDto.ProductId}");

                product.Stock -= itemDto.Quantity;

                var orderItem = new OrderItem
                {
                    ProductId = product.ProductId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = product.Price,
                    Discount = 0
                };

                total += itemDto.Quantity * product.Price;
                order.OrderItems.Add(orderItem);
            }

            decimal discountRate = total > 200 ? 0.10m : total > 100 ? 0.05m : 0;
            foreach (var item in order.OrderItems)
            {
                item.Discount = (item.Quantity * item.UnitPrice) * discountRate;
            }

            order.TotalAmount = order.OrderItems.Sum(i => (i.Quantity * i.UnitPrice) - i.Discount);

            await _orderRepo.AddAsync(order);
            await _orderRepo.SaveChangesAsync();

            var invoice = new Invoice
            {
                OrderId = order.OrderId,
                InvoiceDate = DateTime.UtcNow,
                TotalAmount = order.TotalAmount
            };
            await _invoiceRepo.AddAsync(invoice);
            await _invoiceRepo.SaveChangesAsync();

            return MapToResponseDto(order);
        }

        public async Task UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order == null)
                throw new Exception("Order not found");

            order.Status = newStatus;
            await _orderRepo.UpdateAsync(order);
            await _orderRepo.SaveChangesAsync();

            var customer = await _customerRepo.GetByIdAsync(order.CustomerId);
            if (customer == null || string.IsNullOrEmpty(customer.Email))
                throw new Exception("Customer email not found");

            await SendEmailAsync(
                customer.Email,
                $"Order #{order.OrderId} Status Update",
                $"Dear {customer.Name},\n\nYour order #{order.OrderId} status has been updated to: {newStatus}.\n\nThanks!"
            );
        }

        public OrderResponseDto MapToResponseDto(Order order)
        {
            return new OrderResponseDto
            {
                OrderId = order.OrderId,
                CustomerId = order.CustomerId,
                PaymentMethod = order.PaymentMethod,
                OrderDate = order.OrderDate,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                OrderItems = order.OrderItems.Select(item => new OrderItemResponseDto
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Discount = item.Discount
                }).ToList()
            };
        }
        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            using var smtp = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587)
            {
                //here enter your gmail and app password
                Credentials = new System.Net.NetworkCredential("enter_your_gmail", "enter_your_app_password"),
                EnableSsl = true
            };

            var mailMessage = new System.Net.Mail.MailMessage
            {
                //here enter your gmail
                From = new System.Net.Mail.MailAddress("enter_your_gmail", "Order Management System"),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };

            mailMessage.To.Add(toEmail);

            await smtp.SendMailAsync(mailMessage);
        }

    }
}
