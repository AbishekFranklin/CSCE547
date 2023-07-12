using System;
using OnlineShopping.Models;

namespace OnlineShopping.Model
{
	public class OrderDetail
	{
        public int Id { get; set; }

        public int Quantity { get; set; }

        public int OrderId { get; set; }

        public Order? Order { get; set; }

        public Products? Product { get; set; }
    }
}

