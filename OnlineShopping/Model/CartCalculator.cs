using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineShopping.Models
{
    public class CartCalculator
    {
        // Calculates the total cost of all the items in the shopping cart
        public decimal CalculateRegularTotal(Cart cart)
        {
            decimal regularTotal = 0;

            // Iterate over each item in the shopping cart
            foreach (var item in cart.Items)
            {
                // Calculate the item cost by multiplying the quantity with the price
                decimal itemCost = item.Value * item.Quantity;
                // Add the item cost to the regular total
                regularTotal += itemCost;
            }

            return regularTotal;
        }

        // Calculates the bundle cost of the items in the shopping cart
        public decimal CalculateRegularOrBundleTotal(Cart cart)
        {
            decimal regularTotal = 0m;
            decimal discountedTotal = 0m;

            // Calculates the bundle cost of the items in the shopping cart
            foreach (var item in cart.Items)
            {
                decimal itemCost = item.Value * item.Quantity;
                regularTotal += itemCost;

                // Check if the quantity is greater than and  a multiple of 5
                if (item.Quantity >= 5 || item.Quantity % 5 == 0)
                {
                    // Calculate the discount and subtract it from the regular total
                    decimal discount = itemCost * 0.15m; // 15% discount
                    discountedTotal += itemCost - discount;
                }
                else
                {
                    discountedTotal += itemCost; // No bundle discount
                }
            }

            discountedTotal = decimal.Round(discountedTotal, 2);

            return discountedTotal;
        }

        // Calculates the total cost including tax
        public decimal CalculateTotalWithTaxes(Cart cart)
        {
            decimal totalCost = CalculateRegularOrBundleTotal(cart);
            decimal taxRate = 0.10m;  // Add taxes (10% tax rate)

            // Calculate the tax amount
            decimal taxes = totalCost * taxRate;

            // Calculate the total with taxes
            totalCost += taxes;

            totalCost = decimal.Round(totalCost, 2);

            return totalCost;
        }
    }
}