using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShopping.Model
{
    public class Inventory
    {
        [Key]
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        // Navigation property to link the product
        [Required]
        public virtual Products Product { get; set; }
    }
}