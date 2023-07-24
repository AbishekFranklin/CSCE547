using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Cart name is required.")]
        public string Name { get; set; }

        public List<CartItem> Items { get; set; }

        public Cart()
        {
            Items = new List<CartItem>();
        }
    }

    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        public required string ProductName { get; set; }

        public int Quantity { get; set; }
        public decimal Value { get; set; }

        public int CartId { get; set; }
        public required Cart Cart { get; set; }
    }
}
