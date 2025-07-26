namespace OrderManagementSystem.OrderDTO
{
    public class OrderDto
    {
        public int CustomerId { get; set; }

        public List<OrderItemDto> OrderItems { get; set; } = new();

        public string PaymentMethod { get; set; }
    }
    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
