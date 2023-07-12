using System;
using OnlineShopping.Models;

namespace OnlineShopping.Model
{
	public class Products
	{
		public int Id { get; set; }

		public int ProductId { get; set; }

		public int CategoryId { get; set; }

		public Products? Products { get; set; }

        public Categories? Categories { get; set; }
    }
}

