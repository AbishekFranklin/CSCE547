using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using OnlineShopping.Models;
using OnlineShopping.Services;
using OnlineShopping.InputModel;

namespace OnlineShopping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingController : ControllerBase
    {
        private readonly IShoppingService _shoppingService;

        // Constructor for the ShoppingController class, which receives the IShoppingService.
        public ShoppingController(IShoppingService shoppingService)
        {
            _shoppingService = shoppingService;
        }

        // Action method to get all items from the shopping service.
        [HttpGet("GetAllItems")]
        public IActionResult GetAllItems()
        {
            return _shoppingService.GetAllItems();
        }

        // Action method to create a new cart using the provided cart name.
        [HttpPost("CreateNewCart")]
        public IActionResult CreateNewCart([FromBody] CreateCartInputModel inputModel)
        {
            string cartName = inputModel.CartName;
            return _shoppingService.CreateNewCart(cartName);
        }

        // Action method to add an item to the cart with the provided cart ID, item ID, and quantity.
        [HttpPost("AddItemToCart")]
        public IActionResult AddItemToCart([FromBody] AddItemToCartInputModel inputModel)
        {
            int cartId = inputModel.CartId;
            int itemId = inputModel.ItemId;
            int quantity = inputModel.Quantity;
            return _shoppingService.AddItemToCart(cartId, itemId, quantity);
        }

        // Action method to remove an item from the cart with the provided cart ID and item ID.
        [HttpPost("RemoveItemFromCart")]
        public IActionResult RemoveItemFromCart([FromBody] RemoveItemFromCartInputModel inputModel)
        {
            int cartId = inputModel.CartId;
            int itemId = inputModel.ItemId;
            return _shoppingService.RemoveItem(cartId, itemId);
        }

        // Action method to get the cart details with the provided cart ID.
        [HttpGet("GetCart")]
        public IActionResult GetCart(int cartId)
        {
            return _shoppingService.GetCart(cartId);
        }

        // Action method to get the total cost of the cart with the provided cart ID.
        [HttpGet("GetTotal")]
        public IActionResult GetTotal(int cartId)
        {
            return _shoppingService.GetTotal(cartId);
        }

        // Action method to process the payment for the cart with the provided cart ID and card information.
        [HttpPost("ProcessPayment")]
        public IActionResult ProcessPayment([FromBody] ProcessPaymentInputModel inputModel)
        {
            int cartId = inputModel.CartId;
            CardInformation cardInfo = inputModel.CardInfo;
            return _shoppingService.ProcessPayment(cartId, cardInfo);
        }
    }
}
