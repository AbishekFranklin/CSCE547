using System;
using OnlineShopping.Models;

namespace OnlineShopping.Model
{
	public class Categories
	{
		public int CategoryId { get; set; }

        public required string Name { get; set; }

        public string? Type { get; set; }

        public required ICollection<Products> Products { get; set; }
    }
}

