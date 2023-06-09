﻿using System;

namespace OnlineShopping.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }

        public string? ProductName { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal ItemCost { get; set; }
    }

    public class AddToCartInputModel
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }
    }
}

