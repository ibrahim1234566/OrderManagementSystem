namespace OrderManagementSystem.OrderDTO
{
    public class OrderResponseDto
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItemResponseDto> OrderItems { get; set; } = new();
    }

    public class OrderItemResponseDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
    }
}
