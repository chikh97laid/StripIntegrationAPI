using System.ComponentModel.DataAnnotations;

namespace stripIntegration.Dtos
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public int OrderNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public string OrderStatus { get; set; } = string.Empty;
        public DateTime? PaidAt { get; set; }
        public DateTime? RefundedAt { get; set; }
        public List<OrderItemResponseDto> Items { get; set; } = new();
    }

    public class OrderItemResponseDto
    {
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; } // Quantity * UnitPrice
    }
}
