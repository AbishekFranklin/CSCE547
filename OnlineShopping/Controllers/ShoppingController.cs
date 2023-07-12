using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using OnlineShopping.Model;
using OnlineShopping.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection;
using OnlineShopping.Services;

namespace OnlineShopping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // The ShoppingController handles shopping-related actions
    public class ShoppingController : ControllerBase
    {
        // Declares a private read-only field named _shoppingService of type IShoppingService.
        private readonly IShoppingService _shoppingService;

        // Constructor injection of the shopping service
        public ShoppingController(IShoppingService shoppingService)
        {
            _shoppingService = shoppingService;
        }

        // Retrieves all items from the shopping service and returns them
        [HttpGet("GetAllItems")]
        public IActionResult GetAllItems()
        {
            return _shoppingService.GetAllItems();
        }

        // Adds items to the cart based on the input model
        [HttpPost("AddItemsToCart")]
        public IActionResult AddItemsToCart([FromBody] AddToCartInputModel inputModel)
        {
            int productId = inputModel.ProductId;
            int quantity = inputModel.Quantity;
            return _shoppingService.AddItemsToCart(productId, quantity);
        }

        // Retrieves the items in the cart and returns them
        [HttpGet("CartItems")]
        public IActionResult CartItems()
        {
            return _shoppingService.CartItems();
        }

        // Retrieves the total cost of items in the cart
        [HttpGet("GetTotal")]
        public IActionResult GetTotal()
        {
            return _shoppingService.GetTotal();
        }

        // Processes the payment using the provided card information
        [HttpPost("ProcessPayment")]
        public IActionResult ProcessPayment([FromBody] CardInformation cardInfo)
        {
            return _shoppingService.ProcessPayment(cardInfo);
        }
    }
}
