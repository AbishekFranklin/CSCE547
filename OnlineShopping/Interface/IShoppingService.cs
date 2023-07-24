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

        // Creates a new cart with the provided cartName
        IActionResult CreateNewCart(string cartName);

        // Removes item from the cart
        IActionResult RemoveItem(int cartId, int itemId);

        // Retrieves the items in the cart
        IActionResult GetCart(int cartId);

        // Adds items to the cart based on the provided cartId, itemId, and quantity
        IActionResult AddItemToCart(int cartId, int itemId, int quantity);

        // Retrieves the total cost of items in the cart
        IActionResult GetTotal(int cartId);

        // Processes the payment using the provided card information
        IActionResult ProcessPayment(int cartId, CardInformation cardInfo);
    }
}
