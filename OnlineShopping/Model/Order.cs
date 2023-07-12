using System;
namespace OnlineShopping.Model
{
	public class Order
	{
        public int OrderId { get; set; }

        public DateTime OrderPlaced { get; set; }

        //public DateTime? OrderDelivered { get; set; }

        public int CustomerId { get; set; }

        public required Customers Customer { get; set; }

        public required ICollection<OrderDetail> OrderDetails { get; set; }
    }
}

