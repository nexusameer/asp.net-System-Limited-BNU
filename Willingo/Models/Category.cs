using System.ComponentModel.DataAnnotations;

namespace Willingo.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        public string ImagePath { get; set; } = string.Empty;

        // Navigation Property (Collection)
        public ICollection<Product>? Products { get; set; }
    }
}
