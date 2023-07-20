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

        public ShoppingController(IShoppingService shoppingService)
        {
            _shoppingService = shoppingService;
        }

        [HttpGet("GetAllItems")]
        public IActionResult GetAllItems()
        {
            return _shoppingService.GetAllItems();
        }

        [HttpPost("CreateNewCart")]
        public IActionResult CreateNewCart([FromBody] CreateCartInputModel inputModel)
        {
            string cartName = inputModel.CartName;
            return _shoppingService.CreateNewCart(cartName);
        }

        [HttpPost("AddItemToCart")]
        public IActionResult AddItemToCart([FromBody] AddItemToCartInputModel inputModel)
        {
            int cartId = inputModel.CartId;
            int itemId = inputModel.ItemId;
            int quantity = inputModel.Quantity;
            return _shoppingService.AddItemToCart(cartId, itemId, quantity);
        }

        [HttpPost("RemoveItemFromCart")]
        public IActionResult RemoveItemFromCart([FromBody] RemoveItemFromCartInputModel inputModel)
        {
            int cartId = inputModel.CartId;
            int itemId = inputModel.ItemId;
            return _shoppingService.RemoveItem(cartId, itemId);
        }

        [HttpGet("GetCart")]
        public IActionResult GetCart(int cartId)
        {
            return _shoppingService.GetCart(cartId);
        }

        [HttpGet("GetTotal")]
        public IActionResult GetTotal(int cartId)
        {
            return _shoppingService.GetTotal(cartId);
        }

        [HttpPost("ProcessPayment")]
        public IActionResult ProcessPayment([FromBody] ProcessPaymentInputModel inputModel)
        {
            int cartId = inputModel.CartId;
            CardInformation cardInfo = inputModel.CardInfo;
            return _shoppingService.ProcessPayment(cartId, cardInfo);
        }
    }
}