using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace stripIntegration.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? FirstName { get; set; }

        [MaxLength(100)]
        public string? LastName { get; set; }

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}".Trim();
        
        public string? Phone { get; set; }

        // Navigation property
        public ICollection<Order>? Orders { get; set; } = new List<Order>();
    }
}
