using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace stripIntegration.Models
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; } = null!;

        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; } = null!;

        public int Quantity { get; set; }

        // Price stored at the time of order (major currency unit)
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        // Computed property - not mapped to DB
        [NotMapped]
        public decimal Total => UnitPrice * Quantity;
    }
}
