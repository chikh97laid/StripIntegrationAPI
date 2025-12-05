using System.ComponentModel.DataAnnotations;
namespace stripIntegration.Dtos
{
    public class CreateCheckoutSessionRequest
    {
        [Required, EmailAddress]
        public string CustomerEmail { get; set; } = string.Empty;
        
        [Required]
        public List<OrderItemDto> Items { get; set; } = new();
        
    }

    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
