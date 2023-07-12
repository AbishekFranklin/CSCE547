using Microsoft.AspNetCore.Mvc;
using OnlineShopping.Model;
using OnlineShopping.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;

namespace OnlineShopping.Services
{
    // Abstraction: Interface defining abstract methods for shopping service
    // The IShoppingService interface defines the contract for shopping-related operations
    public interface IShoppingService
    {
        // Retrieves all items available for shopping
        IActionResult GetAllItems();

        // Adds items to the cart based on the provided productId and quantity
        IActionResult AddItemsToCart(int productId, int quantity);

        // Retrieves the items in the cart
        IActionResult CartItems();

        // Retrieves the total cost of items in the cart
        IActionResult GetTotal();

        // Processes the payment using the provided card information
        IActionResult ProcessPayment(CardInformation cardInfo);
    }
}
