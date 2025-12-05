using System.ComponentModel.DataAnnotations;

namespace stripIntegration.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public byte[]? Image { get; set; }
    }
}
