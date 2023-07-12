using System;
using OnlineShopping.Model;

namespace OnlineShopping.Models
{
	public class Products
	{
        // Primitive data type
        public required int ProductId { get; set; }     // Represents an integer value

        public required string Name { get; set; }       // Represents a sequence of characters

        public required decimal Price { get; set; }     // Represents a decimal value

        // Nullable data type
        public string? Description { get; set; }        // Represents a nullable string value

        public string? ProductImage { get; set; }       // Represents a nullable string value

    }
}

