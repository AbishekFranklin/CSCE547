using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Model
{
    public class ProcessedCart
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public required string Items { get; set; }
    }
}
