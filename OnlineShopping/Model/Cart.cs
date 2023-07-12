using System;
using OnlineShopping.Models;

namespace OnlineShopping.Model
{
    public class Cart
    {
        public Dictionary<Products, int> Items { get; set; }

        public List<int> Quantity { get; set; }

        public List<decimal> Prices { get; set; }

        public Cart()
        {

            Items = new Dictionary<Products, int>();

            Quantity = new List<int>();

            Prices = new List<decimal>();
        }
    }

}

