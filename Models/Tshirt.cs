using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderingSystem.Models
{
    public class TShirt
    {
        public int Id { get; set; } // Primary key
        [Required]
        public string Product { get; set; }
        public string Image { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }
        [Required]
        public decimal TotalPrice => Price * Quantity;
    }

    public class OrderedTShirt
    {
        public int Id { get; set; }

        [Required]
        public string Product { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Required]
        public string Image { get; set; }

        /*[Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }*/

        [Required]
        [DataType(DataType.Currency)]
        public decimal TotalPrice { get; set; }
    }
}
