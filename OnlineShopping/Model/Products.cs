using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OnlineShopping.Model;

namespace OnlineShopping.Model
{
    public class Products
    {
        // Primitive data type
        [Key()]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }     // Represents an integer value

        public required string Name { get; set; }       // Represents a sequence of characters

        public required decimal Price { get; set; }     // Represents a decimal value

        // Nullable data type

        public string? Image { get; set; }       // Represents a nullable string value

    }
}

