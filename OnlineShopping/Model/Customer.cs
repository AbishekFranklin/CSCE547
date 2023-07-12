using System;
namespace OnlineShopping.Model
{
	public class Customers
	{
        public int CustomerId { get; set; }

        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public required string Address { get; set; }

        public required string Phone { get; set; }

        //public ICollection<Order>? Orders { get; set; }
    }
}

