using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace stripIntegration.Models
{
    public enum enOrderStatus { Pending, Processing, Paid, Failed, Refunded }
    public enum enPaymentStatus { Pending, Processing, Paid, Failed, Refunded, Cancelled }

    public class Order
    {
        [Key]
        public int Id { get; set; }

        public int OrderNumber { get; set; }

        public DateTime CreatedAt { get; set; }

        public enOrderStatus OrderStatus { get; set; } = enOrderStatus.Pending;

        public string? StripeSessionId { get; set; }
        public string? PaymentIntentId { get; set; }
        public enPaymentStatus PaymentStatus { get; set; } = enPaymentStatus.Pending;
        public DateTime? PaidAt { get; set; }
        public decimal TotalAmount { get; set; }

        public string? RefundId { get; set; }
        public DateTime? RefundedAt { get; set; }

        [Required]
        public int CustomerId { get; set; }

        // Navigation properties
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; } = null!;

        // Order contains multiple order items
        public ICollection<OrderItem>? OrderItems { get; set; } = new List<OrderItem>();

        // Payment reference removed (model deleted)
    }
}
